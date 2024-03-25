using Sigma.Core.Repositories.Base;
using Sigma.Core.Repositories.Base;
using Microsoft.EntityFrameworkCore;
using Sigma.Data;

namespace Sigma.Core.Repositories
{
    public class Apps_Repositories : Repository<Apps>, IApps_Repositories
    {
        public Apps_Repositories(ApplicationDbContext db) : base(db)
        {
        }
    }
}