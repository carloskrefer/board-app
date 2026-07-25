using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace Auth.Infrastructure.Persistance.Helpers;

public static class OptimisticConcurrencyHelper
{
    public const string VersionPropertyName = "RowVersion";

    public static void UpdateRowVersion(EntityEntry entry)
    {
        entry.Property(VersionPropertyName).CurrentValue = Guid.NewGuid();
    }

    public static void UpdateManyRowVersions(IEnumerable<object> entries, DbContext context)
    {
        foreach (var entry in entries)
        {
            var entityEntry = context.Entry(entry);
            UpdateRowVersion(entityEntry);
        }
    }
}