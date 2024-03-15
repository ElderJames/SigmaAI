using AntSK.Domain.Repositories;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Sigma.Data
{
    public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : IdentityDbContext<ApplicationUser>(options)
    {
        public DbSet<Apis> Apis { get; set; }

        public DbSet<Apps> Apps { get; set; }

        public DbSet<Kmss> Kmss { get; set; }

        public DbSet<KmsDetails> KmsDetails { get; set; }

        public DbSet<AIModels> AIModels { get; set; }
    }
}
