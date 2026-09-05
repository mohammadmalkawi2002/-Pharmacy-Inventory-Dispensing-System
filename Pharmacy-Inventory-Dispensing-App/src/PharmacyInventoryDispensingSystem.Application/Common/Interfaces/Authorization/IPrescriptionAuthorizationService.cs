using PharmacyInventoryDispensingSystem.Domain.Entities.Prescriptions;
using System;
using System.Collections.Generic;
using System.Text;

namespace PharmacyInventoryDispensingSystem.Application.Common.Interfaces.Authorization
{
    public interface IPrescriptionAuthorizationService
    {
        Task<bool> CanAccessAsync(
            Prescription prescription,
            CancellationToken cancellationToken = default);
    }
}
