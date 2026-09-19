using MediatR;
using Microsoft.EntityFrameworkCore;
using PharmacyInventoryDispensingSystem.Application.Common.Extensions;
using PharmacyInventoryDispensingSystem.Application.Common.Interfaces;
using PharmacyInventoryDispensingSystem.Application.Common.Interfaces.Repositories;
using PharmacyInventoryDispensingSystem.Application.Common.Models;
using PharmacyInventoryDispensingSystem.Application.Features.Dispenses.Dtos;
using PharmacyInventoryDispensingSystem.Domain.Common.Results;
using PharmacyInventoryDispensingSystem.Domain.Entities.Dispenses;
using System;
using System.Collections.Generic;
using System.Text;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace PharmacyInventoryDispensingSystem.Application.Features.Dispenses.Queries.GetDispenses
{
    public sealed class GetDispensesQueryHandler(
   IGenericRepository<Dispense> dispenseRepository,
    IUserLookupService userLookupService)
    : IRequestHandler<
        GetDispensesQuery,
        Result<PaginatedList<DispenseResponseDto>>>
    {
        public async Task<Result<PaginatedList<DispenseResponseDto>>> Handle(
            GetDispensesQuery request,
            CancellationToken cancellationToken)
        {
            var query= dispenseRepository.Query()
                .Include(d => d.Prescription)
                    .ThenInclude(p => p.Patient)
                .AsQueryable();


            query = ApplySearch(query, request.SearchTerm);

            query = ApplyFiltering(query, request);

            var orderedQuery = query
                        .OrderByDescending(d => d.DispensedAt)
                        .ThenByDescending(d => d.Id);

            //Pagination:
            var paginatedDispenses=await orderedQuery.ToPaginatedListAsync(
                request.PageNumber,
                request.PageSize,
                cancellationToken);

            //Lookups for Pharmacist Names:
            List<string> userIds=paginatedDispenses.Items
                             .Select(dispense=>dispense.PharmacistId)
                             .Distinct()
                             .ToList();


            IReadOnlyDictionary<string, string> userNames =
              userIds.Count == 0
                  ? new Dictionary<string, string>()
                  : await userLookupService.GetUserNamesByIdsAsync(
                      userIds,
                      cancellationToken);

            // . Map to DTO

            var responseItems = paginatedDispenses.Items.Select(dispense =>
            {
                string dispensedByName = userNames.TryGetValue(dispense.PharmacistId, out string? resolvedName)
                        ? resolvedName
                        : dispense.PharmacistId;

                return new DispenseResponseDto(
                    dispense.Id,
                    dispense.PrescriptionId,
                    dispense.Prescription.PrescriptionNumber,
                    dispense.Prescription.Patient.FullName,
                    dispensedByName,
                    dispense.DispensedAt);
            })
                .ToList();


            return new PaginatedList<DispenseResponseDto>(
                responseItems,
                paginatedDispenses.TotalCount,
                request.PageNumber,
                request.PageSize);

   
        }

        private static IQueryable<Dispense> ApplyFiltering(IQueryable<Dispense> query, GetDispensesQuery request)
        {
            if(request.FromDate.HasValue)
                {

                DateTimeOffset from = new(
                request.FromDate.Value.ToDateTime(TimeOnly.MinValue),
                 TimeSpan.Zero);

                query = query.Where(d => d.DispensedAt >= from);
              }



            if (request.ToDate.HasValue)
            {

                DateTimeOffset toExclusive = new(request.ToDate.Value.AddDays(1).ToDateTime(TimeOnly.MinValue), 
                    TimeSpan.Zero);

                query = query.Where(d => d.DispensedAt < toExclusive);
            }

            return query;
        }

        private static IQueryable<Dispense> ApplySearch(IQueryable<Dispense> query, string? searchTerm)
        {
            if (string.IsNullOrWhiteSpace(searchTerm)) 
            {
                return query;
            }

            string normalizedSearchTerm = searchTerm.Trim();

            return query = query.Where(dispense =>
            dispense.Prescription.PrescriptionNumber.StartsWith(normalizedSearchTerm) ||
            dispense.Prescription.Patient.FullName.Contains(normalizedSearchTerm) ||
            dispense.Prescription.Patient.DocumentId.StartsWith(normalizedSearchTerm));
        }







        

     
    }
}
