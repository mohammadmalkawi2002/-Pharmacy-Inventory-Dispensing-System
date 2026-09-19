using PharmacyInventoryDispensingSystem.Domain.Entities.Patients;
using System;
using System.Collections.Generic;
using System.Text;

namespace PharmacyInventoryDispensingSystem.Application.Common.Interfaces.Repositories
{
    public interface IPatientRepository:IGenericRepository<Patient>
    {
        Task<bool> ExistsByDocumentIdAsync(
            string documentId,
            CancellationToken cancellationToken = default);
    }
}
