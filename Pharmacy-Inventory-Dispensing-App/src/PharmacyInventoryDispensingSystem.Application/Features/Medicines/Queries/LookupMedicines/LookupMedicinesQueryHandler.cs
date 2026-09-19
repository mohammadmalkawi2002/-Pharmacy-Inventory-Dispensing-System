using MediatR;
using Microsoft.EntityFrameworkCore;
using PharmacyInventoryDispensingSystem.Application.Common.Interfaces.Repositories;
using PharmacyInventoryDispensingSystem.Application.Features.Medicines.Dtos;
using PharmacyInventoryDispensingSystem.Domain.Common.Results;
using PharmacyInventoryDispensingSystem.Domain.Entities.Medicines;
using System;
using System.Collections.Generic;
using System.Text;

namespace PharmacyInventoryDispensingSystem.Application.Features.Medicines.Queries.LookupMedicines
{
    public sealed class LookupMedicinesQueryHandler(
     IGenericRepository<Medicine> medicineRepository)
     : IRequestHandler<
         LookupMedicinesQuery,
         Result<List<MedicineLookupDto>>>
    {
        private const int LookupLimit = 20;

        public async Task<Result<List<MedicineLookupDto>>> Handle(
            LookupMedicinesQuery request,
            CancellationToken cancellationToken)
        {
            string normalizedSearchTerm = request.SearchTerm.Trim();

            var medicines = await medicineRepository.Query()
                      .Where(m =>
                          m.IsActive &&
                          (m.Name.Contains(normalizedSearchTerm) || m.Code.StartsWith(normalizedSearchTerm)))
                      .OrderBy(m => m.Name)
                      .ThenBy(m => m.Id)
                      .Take(LookupLimit)
                      .Select(m => new MedicineLookupDto(
                          m.Id, m.Code, m.Name, m.Strength, m.Form, m.StockUnit))
                      .ToListAsync(cancellationToken);

                     return medicines;
        }
    }
}
