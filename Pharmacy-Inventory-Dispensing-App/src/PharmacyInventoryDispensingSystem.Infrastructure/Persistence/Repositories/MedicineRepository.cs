using Microsoft.EntityFrameworkCore;
using PharmacyInventoryDispensingSystem.Application.Common.Interfaces.Repositories;
using PharmacyInventoryDispensingSystem.Domain.Entities.Medicines;
using PharmacyInventoryDispensingSystem.Infrastructure.Persistence.Context;
using System;
using System.Collections.Generic;
using System.Text;

namespace PharmacyInventoryDispensingSystem.Infrastructure.Persistence.Repositories
{
    public class MedicineRepository(AppDbContext context)
        : GenericRepository<Medicine>(context), IMedicineRepository
    {
        public async Task<bool> ExistsByCodeAsync(string code, CancellationToken cancellationToken = default)
        {
            return await DbContext.Medicines
                     .IgnoreQueryFilters()
                     .AnyAsync(m => m.Code == code, cancellationToken);
        }
    }
}
