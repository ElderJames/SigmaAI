using AntDesign;
using Sigma.Models;
using Sigma.Services.Auth;
using Microsoft.AspNetCore.Components;

namespace Sigma.Components.Pages.User
{
    public partial class Login
    {
        private readonly LoginParamsType _model = new LoginParamsType();

        [Inject] public NavigationManager NavigationManager { get; set; }

        [Inject] public MessageService Message { get; set; }

        public async Task HandleSubmit()
        {
            //判断是否管理员
            var loginFailed = await ((SigmaAuthProvider)AuthenticationStateProvider).SignIn(_model.UserName, _model.Password);
            if (loginFailed)
            {
                NavigationManager.NavigateTo("/");
                return;
            }
            else
            {
                Message.Error("账号密码错误", 2);
            }
        }
    }
}