using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sigma.Core.Common
{
    public interface ICurrentUser
    {
        public string UserId { get; }

        public string UserName { get; }

    }
}
