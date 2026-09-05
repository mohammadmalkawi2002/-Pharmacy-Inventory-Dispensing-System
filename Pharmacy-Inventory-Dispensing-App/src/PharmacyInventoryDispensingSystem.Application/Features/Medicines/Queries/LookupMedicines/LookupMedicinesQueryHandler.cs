using MediatR;
using PharmacyInventoryDispensingSystem.Application.Common.Interfaces.Repositories;
using PharmacyInventoryDispensingSystem.Application.Features.Medicines.Dtos;
using PharmacyInventoryDispensingSystem.Domain.Common.Results;
using System;
using System.Collections.Generic;
using System.Text;

namespace PharmacyInventoryDispensingSystem.Application.Features.Medicines.Queries.LookupMedicines
{
    public sealed class LookupMedicinesQueryHandler(
     IMedicineRepository medicineRepository)
     : IRequestHandler<
         LookupMedicinesQuery,
         Result<List<MedicineLookupDto>>>
    {
        private const int LookupLimit = 20;

        public async Task<Result<List<MedicineLookupDto>>> Handle(
            LookupMedicinesQuery request,
            CancellationToken cancellationToken)
        {
            List<MedicineLookupDto> medicines =
                await medicineRepository.SearchForLookupAsync(
                    request.SearchTerm,
                    LookupLimit,
                    cancellationToken);

            return medicines;
        }
    }
}
