using MediatR;
using PharmacyInventoryDispensingSystem.Application.Common.Interfaces;
using PharmacyInventoryDispensingSystem.Application.Common.Interfaces.Repositories;
using PharmacyInventoryDispensingSystem.Application.Common.Models;
using PharmacyInventoryDispensingSystem.Application.Features.Dispenses.Dtos;
using PharmacyInventoryDispensingSystem.Domain.Common.Results;
using System;
using System.Collections.Generic;
using System.Text;

namespace PharmacyInventoryDispensingSystem.Application.Features.Dispenses.Queries.GetDispenses
{
    public sealed class GetDispensesQueryHandler(
    IDispenseRepository dispenseRepository,
    IUserLookupService userLookupService)
    : IRequestHandler<
        GetDispensesQuery,
        Result<PaginatedList<DispenseResponseDto>>>
    {
        public async Task<Result<PaginatedList<DispenseResponseDto>>> Handle(
            GetDispensesQuery query,
            CancellationToken cancellationToken)
        {
            // Load the requested page after applying search and date filters.
            var (dispenses, totalCount) =
                await dispenseRepository.GetPagedAsync(
                    query.SearchTerm,
                    query.FromDate,
                    query.ToDate,
                    query.PageNumber,
                    query.PageSize,
                    cancellationToken);

            // Resolve all dispenser names in one request to avoid N+1 queries.
            List<string> userIds = dispenses
                .Select(dispense => dispense.PharmacistId)
                .Distinct()
                .ToList();

            IReadOnlyDictionary<string, string> userNames =
                userIds.Count == 0
                    ? new Dictionary<string, string>()
                    : await userLookupService.GetUserNamesByIdsAsync(
                        userIds,
                        cancellationToken);

            // Map the dispensing records to lightweight list DTOs.
            List<DispenseResponseDto> responseItems = dispenses
                .Select(dispense =>
                {
                    string dispensedByName = userNames.TryGetValue(
                        dispense.PharmacistId,
                        out string? resolvedName)
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
                totalCount,
                query.PageNumber,
                query.PageSize);
        }
    }
}
