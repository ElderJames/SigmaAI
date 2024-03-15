using AntSK.Core.Repositories.Base;
using AntSK.Domain.Repositories.Base;
using Microsoft.EntityFrameworkCore;
using Sigma.Data;

namespace AntSK.Domain.Repositories
{
    public class AIModels_Repositories : Repository<AIModels>, IAIModels_Repositories
    {
        public AIModels_Repositories(ApplicationDbContext db) : base(db)
        {
        }
    }
}