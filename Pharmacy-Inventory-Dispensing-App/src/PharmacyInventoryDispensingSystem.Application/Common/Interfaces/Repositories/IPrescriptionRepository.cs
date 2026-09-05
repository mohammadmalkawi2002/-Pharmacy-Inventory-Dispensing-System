using PharmacyInventoryDispensingSystem.Domain.Entities.Prescriptions;
using PharmacyInventoryDispensingSystem.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace PharmacyInventoryDispensingSystem.Application.Common.Interfaces.Repositories
{
    public interface IPrescriptionRepository
    {
        Task AddAsync(
        Prescription prescription,
        CancellationToken cancellationToken = default);

        Task<string> GenerateNextPrescriptionNumberAsync(
            CancellationToken cancellationToken = default);
        Task<Prescription?> GetByIdWithDetailsAsync(
            Guid prescriptionId,
            CancellationToken cancellationToken = default);

        void RemoveItem(PrescriptionItem item);

        Task<Prescription?> GetForDispensingAsync(
            Guid prescriptionId,
            string documentId,
            CancellationToken cancellationToken = default);

        Task<Prescription?> LookupAsync(
            string prescriptionNumber,
            string documentId,
            CancellationToken cancellationToken = default);


        Task<(IReadOnlyList<Prescription> Items, int TotalCount)> GetPagedAsync(
         string? searchTerm,
         PrescriptionStatus? status,
         string? doctorId,
         string? sortBy,
         bool isDescending,
         int pageNumber,
         int pageSize,
         CancellationToken cancellationToken = default);

        Task<Prescription?> GetByIdAsync(
            Guid prescriptionId,
            CancellationToken cancellationToken = default);

        Task<Prescription?> GetByIdForCancellationAsync(
            Guid prescriptionId,
            CancellationToken cancellationToken = default);
    }
}
