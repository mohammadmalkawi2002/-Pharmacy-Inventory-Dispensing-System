using MediatR;
using Microsoft.Extensions.Logging;
using PharmacyInventoryDispensingSystem.Application.Common.Interfaces.Repositories;
using PharmacyInventoryDispensingSystem.Application.Common.Models;
using PharmacyInventoryDispensingSystem.Application.Features.Patients.Dtos;
using PharmacyInventoryDispensingSystem.Application.Features.Patients.Mappers;
using PharmacyInventoryDispensingSystem.Domain.Common.Results;
using System;
using System.Collections.Generic;
using System.Text;

namespace PharmacyInventoryDispensingSystem.Application.Features.Patients.Queries.GetArchivedPatients
{
    public sealed class GetArchivedPatientsQueryHandler(
        IPatientRepository patientRepository)
        : IRequestHandler<GetArchivedPatientsQuery, Result<PaginatedList<PatientResponseDto>>>
    {
        public async Task<Result<PaginatedList<PatientResponseDto>>> Handle(GetArchivedPatientsQuery request, CancellationToken cancellationToken)
        {

    var (patients,totalCount)= await patientRepository.GetArchivedPagedAsync(
                request.SearchTerm,
                request.DocumentType,
                request.SortBy,
                request.IsDescending,
                request.PageNumber,
                request.PageSize,
                cancellationToken);

            var patientResponses = patients.ToDtos();

            return new PaginatedList<PatientResponseDto>(
                patientResponses,
                totalCount,
                request.PageNumber,
                request.PageSize);



        }
    }
}
