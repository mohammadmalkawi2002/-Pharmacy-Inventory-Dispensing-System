using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens.Experimental;
using PharmacyInventoryDispensingSystem.Application.Common.Extensions;
using PharmacyInventoryDispensingSystem.Application.Common.Interfaces.Repositories;
using PharmacyInventoryDispensingSystem.Application.Common.Models;
using PharmacyInventoryDispensingSystem.Application.Features.Medicines.Queries.GetMedicines;
using PharmacyInventoryDispensingSystem.Application.Features.Patients.Common;
using PharmacyInventoryDispensingSystem.Application.Features.Patients.Dtos;
using PharmacyInventoryDispensingSystem.Application.Features.Patients.Mappers;
using PharmacyInventoryDispensingSystem.Domain.Common.Results;
using PharmacyInventoryDispensingSystem.Domain.Entities.Medicines;
using PharmacyInventoryDispensingSystem.Domain.Entities.Patients;
using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Linq;

namespace PharmacyInventoryDispensingSystem.Application.Features.Patients.Queries.GetPatients
{
    public sealed class GetPatientsQueryHandler(IGenericRepository<Patient> patientRepository)
        : IRequestHandler<
            GetPatientsQuery,
          Result<PaginatedList<PatientResponseDto>>>
    {

        public async Task<Result<PaginatedList<PatientResponseDto>>> Handle(GetPatientsQuery request, CancellationToken cancellationToken)
        {

            var query = patientRepository.Query();

            query = ApplyFiltering(query, request);

            query = ApplySearch(query, request.SearchTerm);

            var orderedQuery = ApplySorting(query, request.SortBy, request.IsDescending);


            var paginatedPatients = await orderedQuery.ToPaginatedListAsync(
                request.PageNumber,
                request.PageSize,
                cancellationToken);

            //Map to Dtos Extension :

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

                "fullname" =>isDescending
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

        private static IQueryable<Patient> ApplyFiltering(IQueryable<Patient> query, GetPatientsQuery request)
        {
            if (request.DocumentType.HasValue)
            {
                query = request.DocumentType.Value switch
                {
                    PatientDocumentType.Citizen => query.Where(p => p.DocumentId.StartsWith("1")),
                    PatientDocumentType.Resident => query.Where(p => p.DocumentId.StartsWith("2")),
                    _ => query

                };
            }

          
            return query;
        }


    }
     



}
