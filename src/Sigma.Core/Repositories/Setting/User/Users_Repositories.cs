using Sigma.Core.Repositories.Base;
using Microsoft.EntityFrameworkCore;
using Sigma.Data;

namespace Sigma.Core.Repositories
{
    public class Users_Repositories : Repository<Users>, IUsers_Repositories
    {
        public Users_Repositories(ApplicationDbContext db) : base(db)
        {
        }
    }
}