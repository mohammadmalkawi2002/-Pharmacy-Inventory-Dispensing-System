using MediatR;
using PharmacyInventoryDispensingSystem.Application.Common.Extensions;
using PharmacyInventoryDispensingSystem.Application.Common.Interfaces.Repositories;
using PharmacyInventoryDispensingSystem.Application.Common.Models;
using PharmacyInventoryDispensingSystem.Application.Features.Medicines.Dtos;
using PharmacyInventoryDispensingSystem.Application.Features.Medicines.Mappers;
using PharmacyInventoryDispensingSystem.Domain.Common.Results;
using PharmacyInventoryDispensingSystem.Domain.Entities.Medicines;
using PharmacyInventoryDispensingSystem.Domain.Enums;
using System.Threading;
using System.Threading.Tasks;

namespace PharmacyInventoryDispensingSystem.Application.Features.Medicines.Queries.GetMedicines
{
    public sealed class GetMedicinesQueryHandler(IGenericRepository<Medicine> medicineRepository)
        : IRequestHandler<GetMedicinesQuery, Result<PaginatedList<MedicineResponseDto>>>
    {
        public async Task<Result<PaginatedList<MedicineResponseDto>>> Handle(
            GetMedicinesQuery request,
            CancellationToken cancellationToken)
        {
            var query= medicineRepository.Query();
            
            query = ApplyFiltering(query, request);

            query=ApplySearch(query, request.SearchTerm);

           var orderedQuery = ApplySorting(query,request.SortBy,request.IsDescending);

            var paginatedMedicines=await orderedQuery.ToPaginatedListAsync(
                request.PageNumber,
                request.PageSize,
                cancellationToken);


            var medicinesDtos = paginatedMedicines.Items.ToDtos();


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
                     ? query.OrderByDescending(m => m.CreatedAtUtc).ThenByDescending(m => m.Id)
                     : query.OrderBy(m => m.CreatedAtUtc).ThenBy(m => m.Id)
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

        private static IQueryable<Medicine> ApplyFiltering(IQueryable<Medicine> query, GetMedicinesQuery request) 
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
