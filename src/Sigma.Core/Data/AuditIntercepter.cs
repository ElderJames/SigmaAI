using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Sigma.Core.Common;
using Sigma.Core.Repositories.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sigma.Core.Data
{
    public class AuditInterceptor : SaveChangesInterceptor
    {
        private ICurrentUser? _currentUser;

        public AuditInterceptor(ICurrentUser? currentUser = null)
        {
            _currentUser = currentUser;
        }

        public override InterceptionResult<int> SavingChanges(DbContextEventData eventData, InterceptionResult<int> result)
        {
            return HandleSavingChanges(eventData, result);
        }

        public override ValueTask<InterceptionResult<int>> SavingChangesAsync(DbContextEventData eventData, InterceptionResult<int> result, CancellationToken cancellationToken = default)
        {
            return new(HandleSavingChanges(eventData, result));
        }

        private InterceptionResult<int> HandleSavingChanges(DbContextEventData eventData, InterceptionResult<int> result)
        {
            if (eventData.Context is null) return result;

            List<AuditLog> auditLogs = [];

            foreach (var entry in eventData.Context.ChangeTracker.Entries())
            {
                if (entry is not { Entity: AuditLog })
                {
                    auditLogs.Add(new AuditEntry(entry, _currentUser?.UserId, _currentUser?.UserName).ToAuditLog());
                }

                if (entry is not { Entity: EntityBase entity })
                {
                    continue;
                }

                ResetProperty(entry, nameof(entity.CreatedAt));
                ResetProperty(entry, nameof(entity.CreatedBy));
                ResetProperty(entry, nameof(entity.UpdatedAt));
                ResetProperty(entry, nameof(entity.UpdatedBy));
                ResetProperty(entry, nameof(entity.DeletedBy));
                ResetProperty(entry, nameof(entity.DeletedAt));


                switch (entry.State)
                {
                    case EntityState.Deleted:
                        entry.State = EntityState.Modified;
                        entity.DeletedAt = DateTime.Now;
                        entity.DeletedBy = _currentUser?.UserId;
                        break;

                    case EntityState.Modified:
                        entity.UpdatedAt = DateTime.Now;
                        entity.UpdatedBy = _currentUser?.UserId;
                        break;

                    case EntityState.Added:
                        entity.CreatedAt = DateTime.Now;
                        entity.CreatedBy = _currentUser?.UserId;
                        break;

                    default:
                        break;
                }
            }

            eventData.Context.AddRange(auditLogs);

            return result;
        }

        private static void ResetProperty(EntityEntry entry, string propertyName)
        {
            entry.Property(propertyName).CurrentValue = entry.Property(propertyName).OriginalValue;
            entry.Property(propertyName).IsModified = false;
        }
    }
}
