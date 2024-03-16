using AntSK.Core.Repositories.Base;
using AntSK.Domain.Repositories.Base;
using Microsoft.EntityFrameworkCore;
using Sigma.Data;

namespace AntSK.Domain.Repositories
{
    public class Kmss_Repositories : Repository<Kmss>, IKmss_Repositories
    {
        public Kmss_Repositories(ApplicationDbContext db) : base(db)
        {
        }
    }
}