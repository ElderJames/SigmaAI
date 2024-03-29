using AntDesign;
using Sigma.Core.Repositories;
using Microsoft.AspNetCore.Components;
using System.Text.RegularExpressions;

namespace Sigma.Components.Pages.PluginPage
{
    public partial class AddPlugin
    {
        [Parameter]
        public string PluginId { get; set; }

        [Inject]
        protected IPluginRepository _pluginRepository { get; set; }

        [Inject]
        protected NavigationManager NavigationManager { get; set; }

        [Inject]
        protected MessageService? Message { get; set; }

        private Plugin _pluginModel = new Plugin();

        protected override async Task OnInitializedAsync()
        {
            await base.OnInitializedAsync();
            if (!string.IsNullOrEmpty(PluginId))
            {
                //查看
                _pluginModel = _pluginRepository.GetFirst(p => p.Id == PluginId);
            }
        }

        private void HandleSubmit()
        {
            if (string.IsNullOrEmpty(PluginId))
            {
                //新增
                _pluginModel.Id = Guid.NewGuid().ToString();

                if (_pluginRepository.IsAny(p => p.Name == _pluginModel.Name))
                {
                    _ = Message.Error("名称已存在！", 2);
                    return;
                }

                string pattern = @"^[A-Za-z]\w*$"; // 正则表达式模式
                if (!Regex.IsMatch(_pluginModel.Name, pattern))
                {
                    _ = Message.Error("API名称只能是字母、数字、下划线组成，且不能以数字开头！", 2);
                    return;
                }

                _pluginRepository.Insert(_pluginModel);
            }
            else
            {
                //修改

                _pluginRepository.Update(_pluginModel);
            }

            Back();
        }

        private void Back()
        {
            NavigationManager.NavigateTo("/plugins/pluginlist");
        }
    }
}