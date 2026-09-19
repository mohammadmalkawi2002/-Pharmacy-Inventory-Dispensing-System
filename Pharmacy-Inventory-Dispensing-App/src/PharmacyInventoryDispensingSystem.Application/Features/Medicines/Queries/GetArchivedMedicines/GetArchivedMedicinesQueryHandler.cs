using MediatR;
using Microsoft.EntityFrameworkCore;
using PharmacyInventoryDispensingSystem.Application.Common.Extensions;
using PharmacyInventoryDispensingSystem.Application.Common.Interfaces.Repositories;
using PharmacyInventoryDispensingSystem.Application.Common.Models;
using PharmacyInventoryDispensingSystem.Application.Features.Medicines.Dtos;
using PharmacyInventoryDispensingSystem.Application.Features.Medicines.Mappers;
using PharmacyInventoryDispensingSystem.Domain.Common.Results;
using PharmacyInventoryDispensingSystem.Domain.Entities.Medicines;
using System.Threading;
using System.Threading.Tasks;

namespace PharmacyInventoryDispensingSystem.Application.Features.Medicines.Queries.GetArchivedMedicines
{
    public sealed class GetArchivedMedicinesQueryHandler(IGenericRepository<Medicine> medicineRepository)
        : IRequestHandler<GetArchivedMedicinesQuery, Result<PaginatedList<MedicineResponseDto>>>
    {
        public async Task<Result<PaginatedList<MedicineResponseDto>>> Handle(
            GetArchivedMedicinesQuery request,
            CancellationToken cancellationToken)
        {
            // Bypass global query filter to get archived items. Track changes is false by default.
            var query =  medicineRepository.QueryIncludingDeleted()
                        .Where(m => m.IsDeleted);

            //Apply search:

            if (!string.IsNullOrWhiteSpace(request.SearchTerm)) 
            {
                string normalizedSearchTerm= request.SearchTerm.Trim();

                query = query.Where(medicine => medicine.Code.StartsWith(normalizedSearchTerm)
                                                || medicine.Name.Contains(normalizedSearchTerm));
            }
                //Apply sorting:

            IOrderedQueryable<Medicine> orderedQuery = query
                .OrderByDescending(medicine => medicine.DeletedAtUtc)
                .ThenBy(medicine => medicine.Id);



            var paginatedMedicines = await orderedQuery.ToPaginatedListAsync(
                request.PageNumber,
                request.PageSize,
                cancellationToken);


            //Map to Extension dtos:

            var medicinesDtos=paginatedMedicines.Items.ToDtos();


            return new PaginatedList<MedicineResponseDto>(
                medicinesDtos,
                paginatedMedicines.TotalCount,
                request.PageNumber,
                request.PageSize);
        }
    }
}
