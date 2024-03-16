using AntSK.Core.Repositories.Base;
using AntSK.Domain.Repositories.Base;
using Microsoft.EntityFrameworkCore;
using Sigma.Data;

namespace AntSK.Domain.Repositories
{
    public class KmsDetails_Repositories : Repository<KmsDetails>, IKmsDetails_Repositories
    {
        public KmsDetails_Repositories(ApplicationDbContext db) : base(db)
        {
        }
    }
}