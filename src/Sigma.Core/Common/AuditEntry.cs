using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json;

namespace Sigma.Core.Common
{
    public class AuditEntry
    {
        public EntityEntry Entry { get; }
        public AuditType AuditType { get; set; }
        public string AuditUserId { get; set; }
        public string TableName { get; set; }
        public Dictionary<string, object?> KeyValues { get; } = new();
        public Dictionary<string, object?> OldValues { get; } = new();
        public Dictionary<string, object?> NewValues { get; } = new();
        public List<string> ChangedColumns { get; } = new List<string>();
        public string AuditUserName { get; private set; }

        public AuditEntry(EntityEntry entry, string? auditUserId, string? auditUserName)
        {
            Entry = entry;
            AuditUserId = auditUserId;
            AuditUserName = auditUserName ?? string.Empty;
            SetChanges();
        }

        private void SetChanges()
        {
            TableName = Entry.Metadata.GetTableName() ?? "UNKNOWN";
            foreach (PropertyEntry property in Entry.Properties)
            {
                string propertyName = property.Metadata.GetColumnName();
                string dbColumnName = property.Metadata.GetColumnName();

                if (property.Metadata.IsPrimaryKey())
                {
                    KeyValues[propertyName] = property.CurrentValue;
                    continue;
                }

                switch (Entry.State)
                {
                    case EntityState.Added:
                        NewValues[propertyName] = property.CurrentValue;
                        AuditType = AuditType.Create;
                        break;

                    case EntityState.Deleted:
                        OldValues[propertyName] = property.OriginalValue;
                        AuditType = AuditType.Delete;
                        break;

                    case EntityState.Modified:
                        if (property.IsModified)
                        {
                            ChangedColumns.Add(dbColumnName);

                            OldValues[propertyName] = property.OriginalValue;
                            NewValues[propertyName] = property.CurrentValue;
                            AuditType = AuditType.Update;
                        }
                        break;
                }
            }
        }

        public AuditLog ToAuditLog()
        {
            var audit = new AuditLog();

            audit.CreatedAt = DateTime.UtcNow;
            audit.AuditType = AuditType.ToString();
            audit.AuditUserId = AuditUserId;
            audit.AuditUserName = AuditUserName;
            audit.TableName = TableName;
            audit.KeyValues = JsonSerializer.Serialize(KeyValues);
            audit.OldValues = OldValues.Count == 0 ?
                              null : JsonSerializer.Serialize(OldValues);
            audit.NewValues = NewValues.Count == 0 ?
                              null : JsonSerializer.Serialize(NewValues);
            audit.ChangedColumns = ChangedColumns.Count == 0 ?
                                   null : JsonSerializer.Serialize(ChangedColumns);

            return audit;
        }
    }
}