using Sigma.Core.Repositories.Base;
using Sigma.Data;

namespace Sigma.Core.Repositories
{
    public class PluginRepository : Repository<Plugin>, IPluginRepository
    {
        public PluginRepository(ApplicationDbContext db) : base(db)
        {
        }
    }
}