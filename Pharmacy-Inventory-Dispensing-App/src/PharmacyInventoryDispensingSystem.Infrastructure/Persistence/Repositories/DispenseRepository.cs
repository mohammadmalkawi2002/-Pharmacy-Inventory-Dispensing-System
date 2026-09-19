using Microsoft.EntityFrameworkCore;
using PharmacyInventoryDispensingSystem.Application.Common.Interfaces.Repositories;
using PharmacyInventoryDispensingSystem.Domain.Entities.Dispenses;
using PharmacyInventoryDispensingSystem.Infrastructure.Persistence.Context;

namespace PharmacyInventoryDispensingSystem.Infrastructure.Persistence.Repositories;

public class DispenseRepository(AppDbContext context)
    :GenericRepository<Dispense>(context), IDispenseRepository
{
   

    public async Task<Dispense?> GetDispenseDetailsByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        return await DbContext.Dispenses
        .AsNoTracking()
            .Include(d => d.Prescription)
            .ThenInclude(p => p.Patient)
            .Include(d => d.Items)
            .ThenInclude(di => di.PrescriptionItem)
                .ThenInclude(pi => pi.Medicine)
          .AsSplitQuery() 
        .FirstOrDefaultAsync(d => d.Id == id, cancellationToken);
    }
}
