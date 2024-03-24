using AntDesign;
using Sigma.Core.Repositories;
using Sigma.Core.Utils;
using Microsoft.AspNetCore.Components;

namespace Sigma.Components.Pages.Setting.User
{
    public partial class UserInfo:ComponentBase
    {
        [Parameter]
        public string UserNo { get; set; }
        [Inject] protected IUsers_Repositories _users_Repositories { get; set; }
        [Inject] protected MessageService? Message { get; set; }

        [Inject] private NavigationManager NavigationManager { get; set; }

        private Users _userModel = new Users();
        private string _password = "";

        protected override async Task OnInitializedAsync()
        {
            await base.OnInitializedAsync();
            if (!string.IsNullOrEmpty(UserNo))
            {
                _userModel = _users_Repositories.GetFirst(p => p.No == UserNo);
                _password = _userModel.Password;
            }
        }

        private async Task HandleSubmit()
        {

            //修改
            if (_userModel.Password != _password)
            {
                _userModel.Password = PasswordUtil.HashPassword(_userModel.Password);
            }
            _users_Repositories.Update(_userModel);


            _ = Message.Info("保存成功！", 2);
        }

        private async Task Back()
        {
            NavigationManager.NavigateTo("/");
        }
    }
}
