using AntSK.Core.Repositories.Base;
using Microsoft.EntityFrameworkCore;
using Sigma.Data;

namespace AntSK.Domain.Repositories
{
    public class Users_Repositories : Repository<Users>, IUsers_Repositories
    {
        public Users_Repositories(ApplicationDbContext db) : base(db)
        {
        }
    }
}