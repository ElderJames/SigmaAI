using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sigma.Core.Common
{
    public class AuditLog
    {
        public int Id { get; set; }
        public DateTime CreatedAt { get; set; }
        public string AuditType { get; set; }
        public string AuditUserId { get; set; }
        public string AuditUserName { get; set; }
        public string TableName { get; set; }
        public string? KeyValues { get; set; }
        public string? OldValues { get; set; }
        public string? NewValues { get; set; }
        public string? ChangedColumns { get; set; }
    }

    public enum AuditType
    {
        None = 0,
        Create = 1,
        Update = 2,
        Delete = 3
    }
}
