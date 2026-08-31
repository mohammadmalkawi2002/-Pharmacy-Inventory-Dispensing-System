using MediatR;
using PharmacyInventoryDispensingSystem.Application.Common.Interfaces.Repositories;
using PharmacyInventoryDispensingSystem.Application.Common.Models;
using PharmacyInventoryDispensingSystem.Application.Features.Medicines.Dtos;
using PharmacyInventoryDispensingSystem.Application.Features.Medicines.Mappers;
using PharmacyInventoryDispensingSystem.Domain.Common.Results;
using System.Threading;
using System.Threading.Tasks;

namespace PharmacyInventoryDispensingSystem.Application.Features.Medicines.Queries.GetMedicines
{
    public sealed class GetMedicinesQueryHandler(IMedicineRepository medicineRepository)
        : IRequestHandler<GetMedicinesQuery, Result<PaginatedList<MedicineResponseDto>>>
    {
        public async Task<Result<PaginatedList<MedicineResponseDto>>> Handle(
            GetMedicinesQuery request,
            CancellationToken cancellationToken)
        {
            var (medicines, totalCount) = await medicineRepository.GetPagedAsync(
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
