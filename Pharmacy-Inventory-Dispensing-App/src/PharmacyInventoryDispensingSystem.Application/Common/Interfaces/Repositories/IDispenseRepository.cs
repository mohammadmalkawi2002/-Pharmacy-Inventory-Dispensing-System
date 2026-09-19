using PharmacyInventoryDispensingSystem.Domain.Entities.Dispenses;

namespace PharmacyInventoryDispensingSystem.Application.Common.Interfaces.Repositories;

public interface IDispenseRepository : IGenericRepository<Dispense>
{

    Task<Dispense?> GetDispenseDetailsByIdAsync(Guid id, CancellationToken cancellationToken = default);
    
}
