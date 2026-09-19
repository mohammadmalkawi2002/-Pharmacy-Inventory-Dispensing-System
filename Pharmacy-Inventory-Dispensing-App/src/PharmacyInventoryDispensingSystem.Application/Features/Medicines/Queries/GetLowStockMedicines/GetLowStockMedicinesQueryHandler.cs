using MediatR;
using PharmacyInventoryDispensingSystem.Application.Common.Extensions;
using PharmacyInventoryDispensingSystem.Application.Common.Interfaces.Repositories;
using PharmacyInventoryDispensingSystem.Application.Common.Models;
using PharmacyInventoryDispensingSystem.Application.Features.Medicines.Dtos;
using PharmacyInventoryDispensingSystem.Application.Features.Medicines.Mappers;
using PharmacyInventoryDispensingSystem.Application.Features.Medicines.Queries.GetMedicines;
using PharmacyInventoryDispensingSystem.Domain.Common.Results;
using PharmacyInventoryDispensingSystem.Domain.Entities.Medicines;
using System.Collections;
using System.Threading;
using System.Threading.Tasks;

namespace PharmacyInventoryDispensingSystem.Application.Features.Medicines.Queries.GetLowStockMedicines
{
    public sealed class GetLowStockMedicinesQueryHandler(IGenericRepository<Medicine> medicineRepository)
        : IRequestHandler<GetLowStockMedicinesQuery, Result<PaginatedList<MedicineResponseDto>>>
    {
        public async Task<Result<PaginatedList<MedicineResponseDto>>> Handle(
            GetLowStockMedicinesQuery request,
            CancellationToken cancellationToken)
        {
            // 1. Start from the generic IQueryable (AsNoTracking, soft-delete filtered(deleted record is skip))
            var query = medicineRepository.Query();

            // 2.Apply the LOW-STOCK  filter :
            query = query.Where(m => m.QuantityInStock > 0
                                     && m.QuantityInStock <= m.ReorderLevel);

            query = ApplyFiltering(query, request);

            query = ApplySearch(query, request.SearchTerm);

            var orderedQuery = ApplySorting(query,request.SortBy,request.IsDescending);

            var paginatedMedicines=await orderedQuery.ToPaginatedListAsync(
                request.PageNumber,
                request.PageSize,
                cancellationToken);

            //Map to Dtos Extension 
           var medicinesDtos=paginatedMedicines.Items.ToDtos();


            return new PaginatedList<MedicineResponseDto>(
                medicinesDtos,
                paginatedMedicines.TotalCount,
                paginatedMedicines.PageNumber,
                paginatedMedicines.PageSize);




        }



        private static IOrderedQueryable<Medicine> ApplySorting(IQueryable<Medicine> query, string? sortBy, bool isDescending)
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

                _ => isDescending
                     ? query.OrderByDescending(m => m.CreatedAtUtc)
                            .ThenByDescending(m => m.Id)
                     : query.OrderBy(m => m.CreatedAtUtc)
                             .ThenBy(m => m.Id)
            };
        }

        private static IQueryable<Medicine> ApplySearch(IQueryable<Medicine> query, string? searchTerm)
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

        private static IQueryable<Medicine> ApplyFiltering(IQueryable<Medicine> query, GetLowStockMedicinesQuery request)
        {
            if (request.Form.HasValue)
            {
                query = query.Where(medicine => medicine.Form == request.Form.Value);
            }

            if (request.StockUnit.HasValue)
            {
                query = query.Where(medicine => medicine.StockUnit == request.StockUnit.Value);
            }

            if (request.IsActive.HasValue)
            {
                query = query.Where(medicine => medicine.IsActive == request.IsActive.Value);
            }

            return query;
        }
    }
}
