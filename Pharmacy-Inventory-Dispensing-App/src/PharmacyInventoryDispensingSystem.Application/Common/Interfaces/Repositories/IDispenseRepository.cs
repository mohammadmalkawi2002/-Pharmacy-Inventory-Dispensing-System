using PharmacyInventoryDispensingSystem.Domain.Entities.Dispenses;
using System;
using System.Collections.Generic;
using System.Text;

namespace PharmacyInventoryDispensingSystem.Application.Common.Interfaces.Repositories
{
    public interface IDispenseRepository
    {
        Task AddAsync(
            Dispense dispense,
            CancellationToken cancellationToken = default);

        Task<Dispense?> GetByIdWithDetailsAsync(Guid dispenseId,
            CancellationToken cancellationToken = default);

        Task<(IReadOnlyList<Dispense> Items, int TotalCount)> GetPagedAsync(
            string? searchTerm,
            DateOnly? fromDate,
            DateOnly? toDate,
            int pageNumber,
            int pageSize,
            CancellationToken cancellationToken = default);



        /// <summary>
        /// Checks if a dispense record exists for a given prescription ID.
        /// </summary>
        /// <param name="prescriptionId"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<bool> ExistsForPrescriptionAsync(
            Guid prescriptionId,
            CancellationToken cancellationToken);
    }
}
