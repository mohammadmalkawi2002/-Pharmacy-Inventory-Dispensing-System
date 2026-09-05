using Microsoft.EntityFrameworkCore;
using PharmacyInventoryDispensingSystem.Application.Common.Interfaces.Repositories;
using PharmacyInventoryDispensingSystem.Application.Features.Medicines.Dtos;
using PharmacyInventoryDispensingSystem.Domain.Entities.Medicines;
using PharmacyInventoryDispensingSystem.Domain.Enums;
using PharmacyInventoryDispensingSystem.Infrastructure.Persistence.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace PharmacyInventoryDispensingSystem.Infrastructure.Persistence.Repositories
{
    public class MedicineRepository(AppDbContext context) : IMedicineRepository
    {
     
        public async Task AddAsync(
            Medicine medicine,
            CancellationToken cancellationToken = default)
        {
            await context.Medicines.AddAsync(medicine, cancellationToken);
        }

        public async Task<bool> ExistsByCodeAsync(
            string code,
            CancellationToken cancellationToken = default)
        {
            return await context.Medicines
                .IgnoreQueryFilters()
                .AnyAsync(
                    medicine => medicine.Code == code,
                    cancellationToken);
        }




        public async Task<bool> IsReferencedByPrescriptionAsync(
            Guid medicineId,
            CancellationToken cancellationToken = default)
        {
            return await context.Medicines
             .AnyAsync(
            medicine =>
                medicine.Id == medicineId &&
                medicine.PrescriptionItems.Any(),
            cancellationToken);
        }




        public async Task<Medicine?> GetByCodeAsync(
            string code,
            CancellationToken cancellationToken = default)
        {
            return await context.Medicines
                .AsNoTracking()
                .FirstOrDefaultAsync(
                    medicine => medicine.Code == code,
                    cancellationToken);
        }

        public async Task<Medicine?> GetByIdAsync(
            Guid medicineId,
            bool trackChanges = false,
            CancellationToken cancellationToken = default)
        {
            var query = context.Medicines.AsQueryable();

            if (!trackChanges)
            {
                query = query.AsNoTracking();
            }

            return await query.FirstOrDefaultAsync(
                medicine => medicine.Id == medicineId,
                cancellationToken);
        }



        public async Task<IReadOnlyList<Medicine>> GetByIdsAsync(
            IReadOnlyCollection<Guid> medicineIds,
            CancellationToken cancellationToken = default)
        {
                
          return await context.Medicines
                .AsNoTracking()
                .Where(medicine => medicineIds.Contains(medicine.Id))
                .ToListAsync(cancellationToken);

        }


        public async Task<Medicine?> GetByIdIncludingArchivedAsync(
            Guid medicineId,
            CancellationToken cancellationToken = default)
        {
            return await context.Medicines
                .IgnoreQueryFilters()
                .FirstOrDefaultAsync(
                    medicine => medicine.Id == medicineId,
                    cancellationToken);
        }

        public async Task<(IReadOnlyList<Medicine> Items, int TotalCount)> GetPagedAsync(
            string? searchTerm,
            MedicineForm? form,
            StockUnit? StockUnit,
            bool? isActive,
            string? sortBy,
            bool isDescending,
            int pageNumber,
            int pageSize,
            CancellationToken cancellationToken = default)
        {
            var query = context.Medicines.AsNoTracking();

            if (form.HasValue)
            {
                query = query.Where(medicine => medicine.Form == form.Value);
            }

            if (isActive.HasValue)
            {
                query = query.Where(medicine => medicine.IsActive == isActive.Value);
            }

            if (StockUnit.HasValue) 
            { 
                query=query.Where(medicine=>medicine.StockUnit == StockUnit.Value);
            }

            query = ApplySearch(query, searchTerm);

            int totalCount = await query.CountAsync(cancellationToken);

            IOrderedQueryable<Medicine> orderedQuery = ApplySorting(
                query,
                sortBy,
                isDescending);

            IReadOnlyList<Medicine> medicines = await orderedQuery
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync(cancellationToken);

            return (medicines, totalCount);
        }








        public async Task<(IReadOnlyList<Medicine> Items, int TotalCount)> GetLowStockPagedAsync(
            string? searchTerm,
            MedicineForm? form,
            StockUnit? StockUnit,
            bool? isActive,
             string? sortBy,
             bool isDescending,
            int pageNumber,
            int pageSize,
            CancellationToken cancellationToken = default)
        {
            var query = context.Medicines
                .AsNoTracking()
                .Where(medicine => medicine.QuantityInStock > 0 && medicine.QuantityInStock <= medicine.ReorderLevel);

            if (isActive.HasValue)
            {
                query=query.Where(medicine=>medicine.IsActive == isActive.Value);
            }

            if (form.HasValue)
            {
                query = query.Where(medicine => medicine.Form == form.Value);
            }


            if (StockUnit.HasValue)
            {
                query = query.Where(medicine => medicine.StockUnit == StockUnit.Value);

            }


                query =ApplySearch(query, searchTerm);


            int totalCount = await query.CountAsync(cancellationToken);



                IOrderedQueryable<Medicine> orderedQuery = ApplySorting(
                query,
                sortBy,
                isDescending);

                IReadOnlyList<Medicine> medicines =await orderedQuery
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync(cancellationToken);

            return (medicines, totalCount);
        }

        public async Task<(IReadOnlyList<Medicine> Items, int TotalCount)> GetArchivedPagedAsync(
            string? searchTerm,
            int pageNumber,
            int pageSize,
            CancellationToken cancellationToken = default)
        {
            var query = context.Medicines
                .IgnoreQueryFilters()
                .AsNoTracking()
                .Where(medicine => medicine.IsDeleted);


            query = ApplySearch(query, searchTerm);


            int totalCount = await query.CountAsync(cancellationToken);

            IReadOnlyList<Medicine> medicines = await query
                .OrderByDescending(medicine => medicine.DeletedAtUtc )
                .ThenBy(medicine => medicine.Id)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync(cancellationToken);

            return (medicines, totalCount);
        }

        private static IQueryable<Medicine> ApplySearch(
            IQueryable<Medicine> query,
            string? searchTerm)
        {
            if (string.IsNullOrWhiteSpace(searchTerm))
            {
                return query;
            }

            string normalizedSearchTerm = searchTerm.Trim();

            return query.Where(medicine =>
                medicine.Code.StartsWith(normalizedSearchTerm) ||
                medicine.Name.Contains(normalizedSearchTerm));
        }

        private static IOrderedQueryable<Medicine> ApplySorting(
            IQueryable<Medicine> query,
            string? sortBy,
            bool isDescending)
        {
            string normalizedSortBy = sortBy?.Trim().ToLower() ?? "createdatutc";

            return normalizedSortBy switch
            {
                "quantityinstock" => isDescending
                    ? query
                        .OrderByDescending(medicine => medicine.QuantityInStock)
                        .ThenByDescending(medicine => medicine.Id)
                    : query
                        .OrderBy(medicine => medicine.QuantityInStock)
                        .ThenBy(medicine => medicine.Id),

                "createdatutc" => isDescending
                    ? query
                        .OrderByDescending(medicine => medicine.CreatedAtUtc)
                        .ThenByDescending(medicine => medicine.Id)
                    : query
                        .OrderBy(medicine => medicine.CreatedAtUtc)
                        .ThenBy(medicine => medicine.Id),

                _ => query
                    .OrderByDescending(medicine => medicine.CreatedAtUtc)
                    .ThenByDescending(medicine => medicine.Id)
            };
        }

        /// <summary>
        /// this method used in frontend to use when selelct  Medicnies from dropdown in CreatePrescription
        /// </summary>
        /// <param name="searchTerm">by Code Or Name</param>
        /// <param name="limit">the number of Medicnies returned</param>
        /// <param name="cancellationToken"></param>
        /// <returns>Active and Not Archived Medicines </returns>

        public async Task<List<MedicineLookupDto>> SearchForLookupAsync(
            string searchTerm,
            int limit,
            CancellationToken cancellationToken = default)
        {
            string normalizedSearchTerm = searchTerm.Trim();


            return await context.Medicines
                .AsNoTracking()
                .Where(medicine =>
                    medicine.IsActive &&
                    (medicine.Name.Contains(normalizedSearchTerm) ||
                     medicine.Code.StartsWith(normalizedSearchTerm)))
                .OrderBy(medicine => medicine.Name)
                .ThenBy(medicine => medicine.Id)
                .Take(limit)
                .Select(medicine => new MedicineLookupDto(
                    medicine.Id,
                    medicine.Code,
                    medicine.Name,
                    medicine.Strength,
                    medicine.Form,
                    medicine.StockUnit))
                .ToListAsync(cancellationToken);
        }
    }
}
