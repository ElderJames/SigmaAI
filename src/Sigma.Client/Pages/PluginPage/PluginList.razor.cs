using AntDesign;
using Sigma.Core.Repositories;
using Microsoft.AspNetCore.Components;

namespace Sigma.Components.Pages.PluginPage
{
    public partial class PluginList
    {
        private Plugin[] _data = { };

        [Inject]
        protected IPluginRepository _pluginRepository { get; set; }

        [Inject]
        private IConfirmService _confirmService { get; set; }

        protected override async Task OnInitializedAsync()
        {
            await base.OnInitializedAsync();
            await InitData("");
        }

        private async Task InitData(string searchKey)
        {
            var list = new List<Plugin> { new Plugin() };
            List<Plugin> data;
            if (string.IsNullOrEmpty(searchKey))
            {
                data = await _pluginRepository.GetListAsync();
            }
            else
            {
                data = await _pluginRepository.GetListAsync(p => p.Name.Contains(searchKey));
            }

            list.AddRange(data);
            _data = list.ToArray();
            await InvokeAsync(StateHasChanged);
        }

        private void NavigateToAddApp()
        {
            NavigationManager.NavigateTo("/plugins/add");
        }

        private async Task Search(string searchKey)
        {
            await InitData(searchKey);
        }

        private void Info(string id)
        {
            NavigationManager.NavigateTo($"/plugins/add/{id}");
        }

        private async Task Delete(string id)
        {
            var content = "是否确认删除此插件";
            var title = "删除";
            var result = await _confirmService.Show(content, title, ConfirmButtons.YesNo);
            if (result == ConfirmResult.Yes)
            {
                await _pluginRepository.DeleteAsync(id);
                await InitData("");
            }
        }
    }
}