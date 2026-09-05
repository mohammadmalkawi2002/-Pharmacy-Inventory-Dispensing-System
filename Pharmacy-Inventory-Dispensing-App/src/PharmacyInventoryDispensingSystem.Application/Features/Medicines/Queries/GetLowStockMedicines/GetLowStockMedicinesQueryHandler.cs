using MediatR;
using PharmacyInventoryDispensingSystem.Application.Common.Interfaces.Repositories;
using PharmacyInventoryDispensingSystem.Application.Common.Models;
using PharmacyInventoryDispensingSystem.Application.Features.Medicines.Dtos;
using PharmacyInventoryDispensingSystem.Application.Features.Medicines.Mappers;
using PharmacyInventoryDispensingSystem.Domain.Common.Results;
using System.Threading;
using System.Threading.Tasks;

namespace PharmacyInventoryDispensingSystem.Application.Features.Medicines.Queries.GetLowStockMedicines
{
    public sealed class GetLowStockMedicinesQueryHandler(IMedicineRepository medicineRepository)
        : IRequestHandler<GetLowStockMedicinesQuery, Result<PaginatedList<MedicineResponseDto>>>
    {
        public async Task<Result<PaginatedList<MedicineResponseDto>>> Handle(
            GetLowStockMedicinesQuery request,
            CancellationToken cancellationToken)
        {
            var (medicines, totalCount) = await medicineRepository.GetLowStockPagedAsync(
                request.SearchTerm,
                 request.Form,
                 request.StockUnit,
                request.IsActive,
                request.SortBy,
                request.IsDescending,
                request.PageNumber,
                request.PageSize,
                cancellationToken);

            var medicinesResponse = medicines.ToDtos();

            return new PaginatedList<MedicineResponseDto>(
                medicinesResponse,
                totalCount,
                request.PageNumber,
                request.PageSize);
        }
    }
}
