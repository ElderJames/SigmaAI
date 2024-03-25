using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sigma.Core.Domain.Chat
{
    public class ChatHistory
    {
        public string ChatId { get; set; }

        public ChatRoles Role { get; set; }

        public string Content { get; set; }
    }
}
