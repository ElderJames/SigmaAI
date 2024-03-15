using AntSK.Core.Repositories.Base;
using AntSK.Domain.Repositories.Base;
using Microsoft.EntityFrameworkCore;
using Sigma.Data;

namespace AntSK.Domain.Repositories
{
    public class Apis_Repositories : Repository<Apis>, IApis_Repositories
    {
        public Apis_Repositories(ApplicationDbContext db) : base(db)
        {
        }
    }
}