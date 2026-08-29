using MediatR;
using PharmacyInventoryDispensingSystem.Application.Common.Interfaces.Repositories;
using PharmacyInventoryDispensingSystem.Application.Common.Models;
using PharmacyInventoryDispensingSystem.Application.Features.Patients.Dtos;
using PharmacyInventoryDispensingSystem.Application.Features.Patients.Mappers;
using PharmacyInventoryDispensingSystem.Domain.Common.Results;
using System;
using System.Collections.Generic;
using System.Text;

namespace PharmacyInventoryDispensingSystem.Application.Features.Patients.Queries.GetPatients
{
    public sealed class GetPatientsQueryHandler (IPatientRepository patientRepository)
        :IRequestHandler<
            GetPatientsQuery,
          Result<PaginatedList<PatientResponseDto>>>
    {

        public async Task<Result<PaginatedList<PatientResponseDto>>> Handle(GetPatientsQuery request, CancellationToken cancellationToken)
        {
            var (patients, totalCount) = await patientRepository.GetPagedAsync(
                request.SearchTerm,
                request.DocumentType,
                request.SortBy, 
                request.IsDescending, 
                request.PageNumber, 
                request.PageSize,
                cancellationToken);

            //Map from entities to dtos:

            var patientsResponse = patients.ToDtos();

            return new PaginatedList<PatientResponseDto>(
                patientsResponse,
                totalCount,
                request.PageNumber,
                request.PageSize);

        }
    }

}
