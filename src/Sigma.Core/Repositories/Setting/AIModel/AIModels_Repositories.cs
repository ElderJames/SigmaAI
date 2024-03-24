using Sigma.Core.Repositories.Base;
using Sigma.Core.Repositories.Base;
using Microsoft.EntityFrameworkCore;
using Sigma.Data;

namespace Sigma.Core.Repositories
{
    public class AIModels_Repositories : Repository<AIModels>, IAIModels_Repositories
    {
        public AIModels_Repositories(ApplicationDbContext db) : base(db)
        {
        }
    }
}