using Sigma.Core.Repositories.Base;
using Sigma.Core.Repositories.Base;
using Microsoft.EntityFrameworkCore;
using Sigma.Data;

namespace Sigma.Core.Repositories
{
    public class Apis_Repositories : Repository<Apis>, IApis_Repositories
    {
        public Apis_Repositories(ApplicationDbContext db) : base(db)
        {
        }
    }
}