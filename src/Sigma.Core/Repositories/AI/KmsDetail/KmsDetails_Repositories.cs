using Sigma.Core.Repositories.Base;
using Sigma.Core.Repositories.Base;
using Microsoft.EntityFrameworkCore;
using Sigma.Data;

namespace Sigma.Core.Repositories
{
    public class KmsDetails_Repositories : Repository<KmsDetails>, IKmsDetails_Repositories
    {
        public KmsDetails_Repositories(ApplicationDbContext db) : base(db)
        {
        }
    }
}