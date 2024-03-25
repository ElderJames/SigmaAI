using Sigma.Core.Repositories.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sigma.Core.Domain.Chat
{
    public class Chat : EntityBase
    {
        public string AppId { get; set; }

        public string Title { get; set; }

    }
}
