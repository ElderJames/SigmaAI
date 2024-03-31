using Sigma.Core.Repositories.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sigma.Core.Domain.Chat
{
    public class ChatHistory : EntityBase
    {
        public string ChatId { get; set; }

        public ChatRoles Role { get; set; }

        public string Content { get; set; }

        public string? FilePath { get; set; }

        public string? FileName { get; set; }
    }
}