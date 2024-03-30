using AntDesign;
using Sigma.Core.Domain.Service;
using Sigma.Core.Repositories;
using Sigma.Core.Utils;
using Microsoft.AspNetCore.Components;

namespace Sigma.Components.Pages.AppPage
{
    public partial class AddApp
    {
        [Parameter]
        public string AppId { get; set; }

        [Inject]
        protected IApps_Repositories _apps_Repositories { get; set; }

        [Inject]
        protected IKmss_Repositories _kmss_Repositories { get; set; }

        [Inject]
        protected IPluginRepository PluginRepository { get; set; }

        [Inject]
        protected NavigationManager NavigationManager { get; set; }

        [Inject]
        protected MessageService? Message { get; set; }

        [Inject]
        protected IAIModels_Repositories _aimodels_Repositories { get; set; }

        [Inject]
        protected FunctionService _functionService { get; set; }

        private Apps _appModel = new Apps();

        private IEnumerable<string> kmsIds;

        private List<Kmss> _kmsList = new List<Kmss>();

        private IEnumerable<string> _pluginIds = [];

        private List<Plugin> _pluginList = new List<Plugin>();

        private IEnumerable<string> funIds = [];

        public Dictionary<string, string> _funList = new Dictionary<string, string>();

        private List<AIModels> _chatList;
        private List<AIModels> _embeddingList;

        protected override async Task OnInitializedAsync()
        {
            await base.OnInitializedAsync();
            _kmsList = _kmss_Repositories.GetList();
            _pluginList = PluginRepository.GetList();

            _chatList = _aimodels_Repositories.GetList(p => p.IsChat);
            _embeddingList = _aimodels_Repositories.GetList(p => p.IsEmbedding);

            _functionService.SearchMarkedMethods();
            foreach (var func in _functionService.Functions)
            {
                var methodInfo = _functionService.MethodInfos[func.Key];
                _funList.Add(func.Key, methodInfo.Description);
            }

            if (!string.IsNullOrEmpty(AppId))
            {
                //查看
                _appModel = _apps_Repositories.GetFirst(p => p.Id == AppId);
                kmsIds = _appModel.KmsIdList?.Split(",");
                _pluginIds = _appModel.PluginList?.Split(",");
                funIds = _appModel.NativeFunctionList?.Split(",");
            }
        }

        private void HandleSubmit()
        {
            if (kmsIds != null && kmsIds.Count() > 0)
            {
                var kmsList = _kmss_Repositories.GetList(p => kmsIds.Contains(p.Id));
                bool allSameEmbeddingModelID = kmsList.Select(k => k.EmbeddingModelID).Distinct().Count() == 1;
                if (!allSameEmbeddingModelID)
                {
                    _ = Message.Error("同一个应用的知识库的Embedding模型必须相同！", 2);
                    return;
                }
                _appModel.KmsIdList = string.Join(",", kmsIds);
            }

            if (_pluginIds.IsNotNull())
            {
                _appModel.PluginList = string.Join(",", _pluginIds);
            }
            if (funIds.IsNotNull())
            {
                _appModel.NativeFunctionList = string.Join(",", funIds);
            }

            if (string.IsNullOrEmpty(AppId))
            {
                //新增
                _appModel.Id = Guid.NewGuid().ToString();
                //秘钥
                _appModel.SecretKey = "sk-" + Guid.NewGuid().ToString();
                if (_apps_Repositories.IsAny(p => p.Name == _appModel.Name))
                {
                    _ = Message.Error("名称已存在！", 2);
                    return;
                }

                _apps_Repositories.Insert(_appModel);
            }
            else
            {
                //修改
                _apps_Repositories.Update(_appModel);
            }

            //NavigationManager.NavigateTo($"/app/detail/{_appModel.Id}");
            NavigationManager.NavigateTo($"/applist");
        }

        private void Back()
        {
            NavigationManager.NavigateTo("/applist");
        }

        private void NavigateModelList()
        {
            NavigationManager.NavigateTo("/setting/modellist");
        }
    }
}