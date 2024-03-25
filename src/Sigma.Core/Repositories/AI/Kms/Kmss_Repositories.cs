using Sigma.Core.Repositories.Base;
using Sigma.Core.Repositories.Base;
using Microsoft.EntityFrameworkCore;
using Sigma.Data;

namespace Sigma.Core.Repositories
{
    public class Kmss_Repositories : Repository<Kmss>, IKmss_Repositories
    {
        public Kmss_Repositories(ApplicationDbContext db) : base(db)
        {
        }
    }
}