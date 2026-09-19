using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using PharmacyInventoryDispensingSystem.Application.Common.Extensions;
using PharmacyInventoryDispensingSystem.Application.Common.Interfaces.Repositories;
using PharmacyInventoryDispensingSystem.Application.Common.Models;
using PharmacyInventoryDispensingSystem.Application.Features.Patients.Common;
using PharmacyInventoryDispensingSystem.Application.Features.Patients.Dtos;
using PharmacyInventoryDispensingSystem.Application.Features.Patients.Mappers;
using PharmacyInventoryDispensingSystem.Application.Features.Patients.Queries.GetPatients;
using PharmacyInventoryDispensingSystem.Domain.Common.Results;
using PharmacyInventoryDispensingSystem.Domain.Entities.Patients;
using System;
using System.Collections.Generic;
using System.Text;

namespace PharmacyInventoryDispensingSystem.Application.Features.Patients.Queries.GetArchivedPatients
{
    public sealed class GetArchivedPatientsQueryHandler(
       IGenericRepository<Patient> patientRepository)
        : IRequestHandler<GetArchivedPatientsQuery, Result<PaginatedList<PatientResponseDto>>>
    {
        public async Task<Result<PaginatedList<PatientResponseDto>>> Handle(GetArchivedPatientsQuery request, CancellationToken cancellationToken)
        {
            // Bypass global query filter to get archived items. Track changes is false by default.

            var query = patientRepository.QueryIncludingDeleted()
                            .Where(p => p.IsDeleted);



            query = ApplyFiltering(query, request);

            query = ApplySearch(query, request.SearchTerm);

            var orderedQuery = ApplySorting(query, request.SortBy, request.IsDescending);

            var paginatedPatients=await orderedQuery.ToPaginatedListAsync(
                request.PageNumber,
                request.PageSize,
                cancellationToken);

            var patientDtos=paginatedPatients.Items.ToDtos();

            return new PaginatedList<PatientResponseDto>(
                patientDtos,
                paginatedPatients.TotalCount,
                paginatedPatients.PageNumber,
                paginatedPatients.PageSize);
        }


        private static IOrderedQueryable<Patient> ApplySorting(IQueryable<Patient> query, string? sortBy, bool isDescending)
        {
            string normalizedSortBy = sortBy?.Trim().ToLower() ?? "createdatutc";

            return normalizedSortBy switch
            {

                "fullname" => isDescending
               ? query.OrderByDescending(p => p.FullName)
                        .ThenByDescending(p => p.Id)
               : query.OrderBy(p => p.FullName)
                        .ThenBy(p => p.Id),

                _ => isDescending
                     ? query.OrderByDescending(p => p.CreatedAtUtc)
                                .ThenByDescending(p => p.Id)
                     : query.OrderBy(p => p.CreatedAtUtc)
                                .ThenBy(p => p.Id)
            };
        }

        private static IQueryable<Patient> ApplySearch(IQueryable<Patient> query, string? searchTerm)
        {
            if (string.IsNullOrWhiteSpace(searchTerm))
            {
                return query;
            }

            string normalizedSearchTerm = searchTerm.Trim();

            return query.Where(patient =>
             patient.DocumentId.StartsWith(normalizedSearchTerm) ||
             patient.FullName.Contains(normalizedSearchTerm) ||
             patient.PhoneNumber.StartsWith(normalizedSearchTerm));

        }

        private static IQueryable<Patient> ApplyFiltering(IQueryable<Patient> query, GetArchivedPatientsQuery request)
        {
            if (request.DocumentType.HasValue)
            {
                query = request.DocumentType.Value switch
                {
                    PatientDocumentType.Citizen => query.Where(p => p.DocumentId.StartsWith("1")),
                    PatientDocumentType.Resident => query.Where(p => p.DocumentId.StartsWith("2")),
                    _=> query

                };
            }


            return query;
        }

    }
}
