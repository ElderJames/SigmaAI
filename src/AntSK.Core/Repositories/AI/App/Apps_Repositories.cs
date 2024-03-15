using AntSK.Core.Repositories.Base;
using AntSK.Domain.Repositories.Base;
using Microsoft.EntityFrameworkCore;
using Sigma.Data;

namespace AntSK.Domain.Repositories
{
    public class Apps_Repositories : Repository<Apps>, IApps_Repositories
    {
        public Apps_Repositories(ApplicationDbContext db) : base(db)
        {
        }
    }
}