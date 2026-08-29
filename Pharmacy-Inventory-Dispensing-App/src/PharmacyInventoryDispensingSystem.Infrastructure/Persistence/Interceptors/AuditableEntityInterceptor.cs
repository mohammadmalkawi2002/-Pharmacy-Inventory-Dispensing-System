using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using PharmacyInventoryDispensingSystem.Application.Common.Interfaces;
using PharmacyInventoryDispensingSystem.Domain.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace PharmacyInventoryDispensingSystem.Infrastructure.Persistence.Interceptors
{
    public sealed class AuditableEntityInterceptor(ICurrentUser currentUser): SaveChangesInterceptor
    {

        public override InterceptionResult<int> SavingChanges(
            DbContextEventData eventData,
            InterceptionResult<int> result)
        {
            UpdateEntities(eventData.Context);

            return base.SavingChanges(eventData, result);
        }


        public override ValueTask<InterceptionResult<int>> SavingChangesAsync(
            DbContextEventData eventData,
            InterceptionResult<int> result,
            CancellationToken cancellationToken = default)
        {
            UpdateEntities(eventData.Context);

            return base.SavingChangesAsync(
                eventData,
                result,
                cancellationToken);
        }

        private void UpdateEntities(DbContext? context) 
        { 
            if(context is null)
            {
                return;
            }

            context.ChangeTracker.DetectChanges();

            DateTimeOffset utcNow=DateTimeOffset.UtcNow;

            string? currentUserId = currentUser.IsAuthenticated
                                    ?currentUser.Id
                                    :null;

            UpdateAuditInformation(
                context,
                utcNow,
                currentUserId);

            UpdateSoftDeleteInformation(
                context,
                utcNow,
                currentUserId);
        }

        private void UpdateSoftDeleteInformation(DbContext context, DateTimeOffset utcNow, string? currentUserId)
        {
            foreach(var entry in context.ChangeTracker.Entries<SoftDeletableEntity>()) 
            {
                if (entry.State != EntityState.Modified)
                {
                    continue;
                }

                var isDeletedProperty =
                    entry.Property(entity => entity.IsDeleted);

                if (!isDeletedProperty.IsModified)
                {
                    continue;
                }

                bool wasDeleted = isDeletedProperty.OriginalValue;
                bool isDeleted = isDeletedProperty.CurrentValue;

                if (!wasDeleted && isDeleted)
                {
                    entry.Entity.DeletedAtUtc = utcNow;
                    entry.Entity.DeletedBy = currentUserId;
                    entry.Entity.RestoredAtUtc = null;
                }
                else if (wasDeleted && !isDeleted)
                {
                    entry.Entity.RestoredAtUtc = utcNow;
                }

            }
        }

        private void UpdateAuditInformation(DbContext context, DateTimeOffset utcNow, string? currentUserId)
        {
            foreach(var entry in context.ChangeTracker.Entries<AuditableEntity>())
            {
                if (entry.State == EntityState.Added) 
                { 
                    entry.Entity.CreatedAtUtc = utcNow;
                    entry.Entity.CreatedBy = currentUserId;

                }



                if (entry.State == EntityState.Modified) 
                {
                    entry.Entity.UpdatedAtUtc = utcNow;
                    entry.Entity.UpdatedBy = currentUserId;
                }

               
            
            }
            
        }
    }
}
