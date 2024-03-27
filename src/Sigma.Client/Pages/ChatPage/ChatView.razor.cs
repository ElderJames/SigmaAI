using AntDesign;
using Sigma.Core.Domain.Interface;
using Sigma.Core.Repositories;
using Sigma.Core.Utils;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Microsoft.KernelMemory;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Connectors.OpenAI;
using RestSharp;
using System.Text;
using Markdig;
using Sigma.Core.Domain.Model;
using Sigma.Core.Domain.Model.Dto;
using Sigma.Core.Domain.Model.Enum;
using Microsoft.Extensions.Logging;
using Sigma.Core.Domain.Chat;
using Sigma.Client.Services;

namespace Sigma.Components.Pages.ChatPage
{
    public partial class ChatView
    {
        [Parameter]
        [SupplyParameterFromQuery]
        public string ChatId { get; set; }

        [Parameter]
        [SupplyParameterFromQuery]
        public string AppId { get; set; }

        [Inject]
        private MessageService? Message { get; set; }

        [Inject]
        private IApps_Repositories _apps_Repositories { get; set; }

        [Inject]
        private IApis_Repositories _apis_Repositories { get; set; }

        [Inject]
        private IKmss_Repositories _kmss_Repositories { get; set; }

        [Inject]
        private IKmsDetails_Repositories _kmsDetails_Repositories { get; set; }

        [Inject] private IJSRuntime _JSRuntime { get; set; }

        [Inject]
        protected IKernelService _kernelService { get; set; }

        [Inject]
        protected IKMService _kMService { get; set; }

        [Inject]
        private IConfirmService _confirmService { get; set; }

        [Inject]
        private IChatService _chatService { get; set; }

        [Inject]
        private ILogger<Chat> Logger { get; set; }

        [Inject]
        private IChatRepository ChatRepository { get; set; }

        [Inject]
        private LayoutService LayoutService { get; set; }

        private bool _loading = false;

        //private List<MessageInfo> MessageList = [];
        private string? _messageInput;
        private string _json = "";
        private bool Sendding = false;

        private string[] _selectedApps = [];
        private string[] _selectedChat = [];
        private Chat? _chat;
        private Apps? _app;

        private List<RelevantSource> _relevantSources = new List<RelevantSource>();

        private List<Apps> _appList = [];
        private List<Chat> _chatList = [];
        private List<ChatHistory> _histories = [];

        protected override async Task OnInitializedAsync()
        {
            LayoutService.ChangeSiderCollapsed(true);

            await base.OnInitializedAsync();
            _appList = _apps_Repositories.GetList();

            if (_appList.Any())
            {
                _app = _appList.First();
                AppId = _app.Id;

                await OnAppSelectChange([AppId]);
                //_chatList = await ChatRepository.GetListAsync(x => x.AppId == _app.Id);
                //_chat = _chatList.FirstOrDefault();
                //if (_chat != null)
                //{
                //    ChatId = _chat.Id;
                //}
                //else
                //{
                //    ChatId = Guid.NewGuid().ToString();
                //    _chat = new() { AppId = _app.Id, Id = ChatId, Title = "新对话" };

                //    _chatList.Add(_chat);
                //}

                //_selectedChat = [ChatId];
                //_selectedApps = [AppId];

            }
        }

        private async Task OnAppSelectChange(string[] appIds)
        {
            _selectedApps = appIds;
            var appId = appIds.FirstOrDefault();
            if (appId == null)
            {
                return;
            }

            _app = _appList.FirstOrDefault(x => x.Id == appId);
            if (_app == null)
            {
                return;
            }

            _chatList = await ChatRepository.GetListAsync(x => x.AppId == appId);
            _chat = _chatList.FirstOrDefault();
            if (_chat != null)
            {
                ChatId = _chat.Id;
            }
            else
            {
                ChatId = Guid.NewGuid().ToString();
                _chat = new() { AppId = _app.Id, Id = ChatId, Title = "新对话" };

                _chatList.Add(_chat);

            }

            await OnChatSelectChange([ChatId]);
        }

        private async Task OnChatSelectChange(string[] chatIds)
        {
            _selectedChat = chatIds;

            ChatId = chatIds.First();

            _chat = _chatList.FirstOrDefault(x => x.Id == ChatId);

            _histories = await ChatRepository.GetChatHistories(ChatId, 0, 10);
        }

        protected async Task OnSendAsync()
        {
            try
            {
                if (string.IsNullOrWhiteSpace(_messageInput))
                {
                    _ = Message.Info("请输入消息", 2);
                    return;
                }

                if (string.IsNullOrWhiteSpace(AppId))
                {
                    _ = Message.Info("请选择应用进行测试", 2);
                    return;
                }

                if (_chat.CreatedBy == null)
                {
                    _chat.Title = _messageInput[..int.Min(9, _messageInput.Length - 1)];
                    ChatRepository.Insert(_chat);
                }

                //MessageList.Add(new MessageInfo()
                //{
                //    ID = Guid.NewGuid().ToString(),
                //    Context = _messageInput,
                //    CreateTime = DateTime.Now,
                //    IsSend = true
                //});

                var history = new ChatHistory
                {
                    ChatId = ChatId,
                    Role = ChatRoles.User,
                    Content = _messageInput,
                };

                await ChatRepository.CreateHistory(history);
                _histories.Add(history);

                Sendding = true;

                StateHasChanged();

                await SendAsync(_messageInput);
                _messageInput = "";
                Sendding = false;
            }
            catch (System.Exception ex)
            {
                Sendding = false;
                Logger.LogError(ex, "对话异常");
                _ = Message.Error("异常:" + ex.Message, 2);
            }
        }

        protected async Task OnCopyAsync(ChatHistory item)
        {
            await Task.Run(() =>
            {
                _messageInput = item.Content;
            });
        }

        protected async Task OnClearAsync()
        {
            if (_histories.Count > 0)
            {
                var content = "是否要清理会话记录";
                var title = "清理";
                var result = await _confirmService.Show(content, title, ConfirmButtons.YesNo);
                if (result == ConfirmResult.Yes)
                {
                    _histories.Clear();
                    _ = Message.Info("清理成功");
                }
            }
            else
            {
                _ = Message.Info("没有会话记录");
            }
        }

        protected async Task<bool> SendAsync(string questions)
        {
            string msg = "";
            //处理多轮会话
            Apps app = _apps_Repositories.GetFirst(p => p.Id == AppId);
            if (_histories.Count > 0)
            {
                msg = await HistorySummarize(app, questions);
            }

            switch (app.Type)
            {
                case AppType.Chat:
                    //普通会话
                    await SendChat(questions, msg, app);
                    break;

                case AppType.Kms:
                    //知识库问答
                    await SendKms(questions, msg, app);
                    break;
            }

            return await Task.FromResult(true);
        }

        /// <summary>
        /// 发送知识库问答
        /// </summary>
        /// <param name="questions"></param>
        /// <param name="msg"></param>
        /// <param name="app"></param>
        /// <returns></returns>
        private async Task SendKms(string questions, string msg, Apps app)
        {
            var chatResult = _chatService.SendKmsByAppAsync(app, questions, msg, _relevantSources);

            ChatHistory chatHistory = new() { ChatId = ChatId, Role = ChatRoles.Assistant };

            await foreach (var content in chatResult)
            {
                chatHistory.Content += content;

                await Task.Delay(50);

                await InvokeAsync(StateHasChanged);
            }

            //全部处理完后再处理一次Markdown
            await MarkDown(chatHistory);
        }

        /// <summary>
        /// 发送普通对话
        /// </summary>
        /// <param name="questions"></param>
        /// <param name="history"></param>
        /// <param name="app"></param>
        /// <returns></returns>
        private async Task SendChat(string questions, string history, Apps app)
        {
            //MessageInfo info = null;
            var chatResult = _chatService.SendChatByAppAsync(app, questions, history);

            ChatHistory chatHistory = new() { ChatId = ChatId, Role = ChatRoles.Assistant };

            await foreach (var content in chatResult)
            {
                chatHistory.Content += content;

                await Task.Delay(50);

                await InvokeAsync(StateHasChanged);
            }
            //全部处理完后再处理一次Markdown
            await MarkDown(chatHistory);
        }

        private async Task MarkDown(ChatHistory info)
        {
            if (info.IsNotNull())
            {
                // info!.HtmlAnswers = markdown.Transform(info.HtmlAnswers);
                info!.Content = Markdown.ToHtml(info.Content);
            }
            await InvokeAsync(StateHasChanged);
            await _JSRuntime.InvokeVoidAsync("Prism.highlightAll");
            await _JSRuntime.ScrollToBottomAsync("scrollDiv");
        }

        /// <summary>
        /// 历史会话的会话总结
        /// </summary>
        /// <param name="questions"></param>
        /// <returns></returns>
        private async Task<string> HistorySummarize(Apps app, string questions)
        {
            var _kernel = _kernelService.GetKernelByApp(app);
            if (_histories.Count > 1)
            {
                StringBuilder history = new StringBuilder();
                foreach (var item in _histories)
                {
                    if (item.Role == ChatRoles.User)
                    {
                        history.Append($"user:{item.Content}{Environment.NewLine}");
                    }
                    else
                    {
                        history.Append($"assistant:{item.Content}{Environment.NewLine}");
                    }
                }
                if (_histories.Count > 10)
                {
                    //历史会话大于10条，进行总结
                    var msg = await _kernelService.HistorySummarize(_kernel, questions, history.ToString());
                    return msg;
                }
                else
                {
                    var msg = $"history：{history.ToString()}{Environment.NewLine}";
                    return msg;
                }
            }
            else
            {
                return "";
            }
        }
    }
}