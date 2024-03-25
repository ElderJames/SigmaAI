using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Sigma.Core.Common
{
    public class CurrentUser(IHttpContextAccessor httpContextAccessor) : ICurrentUser
    {
        private string _userId = string.Empty;
        private string _userName = string.Empty;

        public string UserId
        {
            get
            {
                if (string.IsNullOrEmpty(_userId))
                {
                    _userId = User?.FindFirstValue(ClaimTypes.NameIdentifier) ?? "";
                }

                return _userId;
            }
        }

        public string UserName
        {
            get
            {
                if (string.IsNullOrEmpty(_userName))
                {
                    _userName = User?.FindFirstValue(ClaimTypes.Name) ?? string.Empty;
                }

                return _userName;
            }
        }

        public ClaimsPrincipal? User => httpContextAccessor.HttpContext?.User;
    }
}