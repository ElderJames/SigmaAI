using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sigma.Core.Repositories.Base
{
    public class EntityBase
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();

        public DateTime CreatedAt { get; set; }

        public string? CreatedBy { get; set; }

        public DateTime UpdatedAt { get; set;}

        public string? UpdatedBy { get; set;}

        public DateTime DeletedAt { get; set; }

        public string? DeletedBy { get; set; }   

        public bool IsDeleted { get; set; } 

        public int TenantId { get; set; }
    }
}