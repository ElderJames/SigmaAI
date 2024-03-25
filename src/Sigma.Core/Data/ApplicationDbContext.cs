using Sigma.Core.Repositories;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Sigma.Core.Domain.Chat;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Sigma.Core.Data;
using Sigma.Core.Common;

namespace Sigma.Data
{
    public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options, AuditInterceptor auditInterceptor) : IdentityDbContext<ApplicationUser>(options)
    {
        public DbSet<Apis> Apis { get; set; }

        public DbSet<Apps> Apps { get; set; }

        public DbSet<Kmss> Kmss { get; set; }

        public DbSet<KmsDetails> KmsDetails { get; set; }

        public DbSet<AIModels> AIModels { get; set; }

        public DbSet<Users> Admin { get; set; }

        public DbSet<Chat> Chats { get; set; }

        public DbSet<AuditLog> AuditLogs { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<Apis>().HasQueryFilter(x => !x.IsDeleted);
            builder.Entity<Apps>().HasQueryFilter(x => !x.IsDeleted);
            builder.Entity<Kmss>().HasQueryFilter(x => !x.IsDeleted);
            builder.Entity<KmsDetails>().HasQueryFilter(x => !x.IsDeleted);
            builder.Entity<AIModels>().HasQueryFilter(x => !x.IsDeleted);
            builder.Entity<Users>().HasQueryFilter(x => !x.IsDeleted);
            builder.Entity<Chat>().HasQueryFilter(x => !x.IsDeleted);

            base.OnModelCreating(builder);
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.AddInterceptors(auditInterceptor);
            base.OnConfiguring(optionsBuilder);
        }
    }
}