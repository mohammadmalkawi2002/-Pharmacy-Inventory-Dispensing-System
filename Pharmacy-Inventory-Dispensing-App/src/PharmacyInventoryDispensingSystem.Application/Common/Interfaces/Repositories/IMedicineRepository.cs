using PharmacyInventoryDispensingSystem.Domain.Entities.Medicines;
using PharmacyInventoryDispensingSystem.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace PharmacyInventoryDispensingSystem.Application.Common.Interfaces.Repositories
{
    public interface IMedicineRepository
    {
        Task AddAsync(
            Medicine medicine,
            CancellationToken cancellationToken = default);

        Task<Medicine?> GetByIdAsync(
            Guid medicineId,
            bool trackChanges = false,
            CancellationToken cancellationToken = default);

        Task<Medicine?> GetByIdIncludingArchivedAsync(
            Guid medicineId,
            CancellationToken cancellationToken = default);

        Task<Medicine?> GetByCodeAsync(
            string code,
            CancellationToken cancellationToken = default);

        Task<bool> ExistsByCodeAsync(
            string code,
            CancellationToken cancellationToken = default);

        Task<bool> IsReferencedByPrescriptionAsync(
            Guid medicineId,
            CancellationToken cancellationToken = default);

        Task<(IReadOnlyList<Medicine> Items, int TotalCount)> GetPagedAsync(
            string? searchTerm,
            MedicineForm? form,
            StockUnit? StockUnit,
            bool? isActive,
            string? sortBy,
            bool isDescending,
            int pageNumber,
            int pageSize,
            CancellationToken cancellationToken = default);

        Task<(IReadOnlyList<Medicine> Items, int TotalCount)> GetLowStockPagedAsync(
             string? searchTerm,
             bool? isActive,
             int pageNumber,
             int pageSize,
             CancellationToken cancellationToken = default);

        Task<(IReadOnlyList<Medicine> Items, int TotalCount)> GetArchivedPagedAsync(
            string? searchTerm,
            int pageNumber,
            int pageSize,
            CancellationToken cancellationToken = default);
    }
}
