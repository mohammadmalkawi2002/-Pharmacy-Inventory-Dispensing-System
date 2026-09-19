using PharmacyInventoryDispensingSystem.Application.Common.Interfaces.Repositories;
using PharmacyInventoryDispensingSystem.Infrastructure.Persistence.Context;
using System;
using System.Collections.Generic;
using System.Text;

namespace PharmacyInventoryDispensingSystem.Infrastructure.Persistence.Repositories
{
    public sealed class UnitOfWork(AppDbContext context) : IUnitOfWork
    {
        

        public async Task SaveChangesAsync(CancellationToken cancellationToken = default)
        {
              await context.SaveChangesAsync(cancellationToken);
        }
    }
}
