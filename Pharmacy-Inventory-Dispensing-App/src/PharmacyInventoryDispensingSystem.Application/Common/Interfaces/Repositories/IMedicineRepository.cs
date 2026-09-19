using PharmacyInventoryDispensingSystem.Domain.Entities.Medicines;
using System;
using System.Collections.Generic;
using System.Text;

namespace PharmacyInventoryDispensingSystem.Application.Common.Interfaces.Repositories
{
    public interface IMedicineRepository : IGenericRepository<Medicine>
    {
        Task<bool> ExistsByCodeAsync(string code, CancellationToken cancellationToken = default);
    }
}
