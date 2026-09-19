using Microsoft.EntityFrameworkCore;
using PharmacyInventoryDispensingSystem.Application.Common.Interfaces.Repositories;
using PharmacyInventoryDispensingSystem.Domain.Entities.Patients;
using PharmacyInventoryDispensingSystem.Infrastructure.Persistence.Context;
using System;
using System.Collections.Generic;
using System.Text;

namespace PharmacyInventoryDispensingSystem.Infrastructure.Persistence.Repositories
{
    public class PatientRepository(AppDbContext context)
        : GenericRepository<Patient>(context), IPatientRepository
    {
        public async Task<bool> ExistsByDocumentIdAsync(string documentId, CancellationToken cancellationToken = default)
        {
            return await DbContext.Patients
                .IgnoreQueryFilters()
                .AnyAsync(p => p.DocumentId == documentId, cancellationToken);
        }
    }
}
