using MediatR;
using Microsoft.EntityFrameworkCore;
using PharmacyInventoryDispensingSystem.Application.Common.Extensions;
using PharmacyInventoryDispensingSystem.Application.Common.Interfaces;
using PharmacyInventoryDispensingSystem.Application.Common.Interfaces.Repositories;
using PharmacyInventoryDispensingSystem.Application.Common.Models;
using PharmacyInventoryDispensingSystem.Application.Features.Prescriptions.Dtos;
using PharmacyInventoryDispensingSystem.Application.Features.Prescriptions.Mappers;
using PharmacyInventoryDispensingSystem.Application.Features.SecurityManager.Authorization;
using PharmacyInventoryDispensingSystem.Domain.Common.Results;
using PharmacyInventoryDispensingSystem.Domain.Entities.Prescriptions;
using System;
using System.Collections.Generic;
using System.Text;

namespace PharmacyInventoryDispensingSystem.Application.Features.Prescriptions.Queries.GetPrescriptions
{
    public sealed class GetPrescriptionsQueryHandler(
     IGenericRepository<Prescription> prescriptionRepository,
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

            var query = prescriptionRepository.Query()
                  .Include(p => p.Patient)
                  .AsQueryable();

            // Doctor ownership filter:
            if (!string.IsNullOrWhiteSpace(doctorId))
                query = query.Where(p => p.DoctorId == doctorId);

            //Apply Filtering:
            if (request.Status.HasValue)
                query = query.Where(p => p.Status == request.Status.Value);

            //Apply Searching:
            query = ApplySearch(query, request.SearchTerm);

            //Apply Sorting:

            var orderedQuery = ApplySorting(
                query,
                request.SortBy,
                request.IsDescending);

            var paginatedPrescriptions = await orderedQuery.ToPaginatedListAsync(
                request.PageNumber,
                request.PageSize,
                cancellationToken);


            // Get the distinct doctorIds from the prescriptions
            var doctorIds = paginatedPrescriptions.Items
                        .Select(prescription => prescription.DoctorId)
                        .Distinct()
                        .ToList();

            // Fetch the doctor names for the distinct doctorIds
            var doctorNames = await userLookupService.GetUserNamesByIdsAsync(
                doctorIds,
                cancellationToken);


            // Map the prescriptions to PrescriptionSummaryDto, including the doctor names:
            var prescriptionDtos = paginatedPrescriptions.Items.ToSummaryDtos(doctorNames);

            return new PaginatedList<PrescriptionSummaryDto>(
                prescriptionDtos,
                paginatedPrescriptions.TotalCount,
                paginatedPrescriptions.PageNumber,
                paginatedPrescriptions.PageSize);


        }



        private static IQueryable<Prescription> ApplySearch(
            IQueryable<Prescription> query,
            string? searchTerm)
        {
            if (string.IsNullOrWhiteSpace(searchTerm))
            {
                return query;
            }

            string normalizedSearchTerm = searchTerm.Trim();

            return query.Where(prescription =>
                prescription.PrescriptionNumber.StartsWith(normalizedSearchTerm) ||
                prescription.Patient.FullName.Contains(normalizedSearchTerm) ||
                prescription.Patient.DocumentId.StartsWith(normalizedSearchTerm));
        }

        private static IOrderedQueryable<Prescription> ApplySorting(
            IQueryable<Prescription> query,
            string? sortBy,
            bool isDescending)
        {
            string normalizedSortBy =
                sortBy?.Trim().ToLower() ?? "createdatutc";

            return normalizedSortBy switch
            {
                "prescriptionnumber" => isDescending
                    ? query
                        .OrderByDescending(prescription => prescription.PrescriptionNumber)
                        .ThenByDescending(prescription => prescription.Id)
                    : query
                        .OrderBy(prescription => prescription.PrescriptionNumber)
                        .ThenBy(prescription => prescription.Id),

                "validfrom" => isDescending
                    ? query
                        .OrderByDescending(prescription => prescription.ValidFrom)
                        .ThenByDescending(prescription => prescription.Id)
                    : query
                        .OrderBy(prescription => prescription.ValidFrom)
                        .ThenBy(prescription => prescription.Id),

                "validto" => isDescending
                    ? query
                        .OrderByDescending(prescription => prescription.ValidTo)
                        .ThenByDescending(prescription => prescription.Id)
                    : query
                        .OrderBy(prescription => prescription.ValidTo)
                        .ThenBy(prescription => prescription.Id),

                _ => isDescending
                    ? query.OrderByDescending(p => p.CreatedAtUtc)
                           .ThenByDescending(p => p.Id)
                    : query.OrderBy(p => p.CreatedAtUtc)
                            .ThenBy(p => p.Id)

            };
        }
    }
}
