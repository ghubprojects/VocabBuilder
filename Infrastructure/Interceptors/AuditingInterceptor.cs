using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Diagnostics;
using VocabBuilder.Infrastructure.Entities;

namespace VocabBuilder.Infrastructure.Interceptors;

public class AuditingInterceptor(string currentUser = "System") : SaveChangesInterceptor
{
    public override async ValueTask<InterceptionResult<int>> SavingChangesAsync(DbContextEventData eventData, InterceptionResult<int> result, CancellationToken cancellationToken = default)
    {
        var context = eventData.Context;
        if (context is null)
            return await base.SavingChangesAsync(eventData, result, cancellationToken);

        var now = DateTime.Now;
        foreach (var entry in context.ChangeTracker.Entries<IAuditable>())
        {
            switch (entry.State)
            {
                case EntityState.Added:
                    SetCreationAuditInfo(entry.Entity, currentUser, now);
                    break;

                case EntityState.Modified:
                    SetModificationAuditInfo(entry.Entity, currentUser, now);
                    break;

                case EntityState.Deleted:
                    SetDeletionAuditInfo(entry, currentUser, now);
                    break;
            }
        }

        return await base.SavingChangesAsync(eventData, result, cancellationToken);
    }

    private static void SetCreationAuditInfo(IAuditable entity, string currentUser, DateTime now)
    {
        entity.CreatedAt = now;
        entity.CreatedBy = currentUser;
    }

    private static void SetModificationAuditInfo(IAuditable entity, string currentUser, DateTime now)
    {
        entity.UpdatedAt = now;
        entity.UpdatedBy = currentUser;
    }

    private static void SetDeletionAuditInfo(EntityEntry entry, string currentUser, DateTime now)
    {
        if (entry.Entity is ISoftDeletable softDelete)
        {
            softDelete.IsDeleted = true;
            softDelete.DeletedAt = now;
            softDelete.DeletedBy = currentUser;
            entry.State = EntityState.Modified;
        }
    }
}