using MediatR;
using PharmacyInventoryDispensingSystem.Application.Common.Interfaces;
using PharmacyInventoryDispensingSystem.Application.Common.Interfaces.Repositories;
using PharmacyInventoryDispensingSystem.Application.Common.Models;
using PharmacyInventoryDispensingSystem.Application.Features.Prescriptions.Dtos;
using PharmacyInventoryDispensingSystem.Application.Features.Prescriptions.Mappers;
using PharmacyInventoryDispensingSystem.Application.Features.SecurityManager.Authorization;
using PharmacyInventoryDispensingSystem.Domain.Common.Results;
using System;
using System.Collections.Generic;
using System.Text;

namespace PharmacyInventoryDispensingSystem.Application.Features.Prescriptions.Queries.GetPrescriptions
{
    public sealed class GetPrescriptionsQueryHandler(
    IPrescriptionRepository prescriptionRepository,
    IUserLookupService userLookupService,
    ICurrentUser currentUser)
    : IRequestHandler<GetPrescriptionsQuery, Result<PaginatedList<PrescriptionSummaryDto>>>
    {
        public async Task<Result<PaginatedList<PrescriptionSummaryDto>>> Handle(
            GetPrescriptionsQuery request,
            CancellationToken cancellationToken)
        {
            // Determine the doctorId based on the current user's role(if the user is an admin,
            // doctorId will be null, otherwise it will be the current user's Id)
            string? doctorId = currentUser.IsInRole(RoleNames.Admin)
                                ? null
                                : currentUser.Id;

            var (prescriptions, totalCount) = await prescriptionRepository.GetPagedAsync(
                        request.SearchTerm,
                        request.Status,
                        doctorId,
                        request.SortBy,
                        request.IsDescending,
                        request.PageNumber,
                        request.PageSize,
                        cancellationToken);


            // Get the distinct doctorIds from the prescriptions
            var doctorIds = prescriptions
                        .Select(prescription => prescription.DoctorId)
                        .Distinct()
                        .ToList();

            // Fetch the doctor names for the distinct doctorIds
            var doctorNames = await userLookupService.GetUserNamesByIdsAsync(
                doctorIds,
                cancellationToken);


            // Map the prescriptions to PrescriptionSummaryDto, including the doctor names:
            var prescriptionsResponse = prescriptions.ToSummaryDtos(doctorNames);

            // Create a PaginatedList<PrescriptionSummaryDto> to return:

            return new PaginatedList<PrescriptionSummaryDto>(
                prescriptionsResponse,
                totalCount,
                request.PageNumber,
                request.PageSize);



        }
    }
}
