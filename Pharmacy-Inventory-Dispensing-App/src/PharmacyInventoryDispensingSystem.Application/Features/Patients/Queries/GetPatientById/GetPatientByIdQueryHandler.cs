using MediatR;
using Microsoft.Extensions.Logging;
using PharmacyInventoryDispensingSystem.Application.Common.Errors;
using PharmacyInventoryDispensingSystem.Application.Common.Interfaces.Repositories;
using PharmacyInventoryDispensingSystem.Application.Features.Patients.Dtos;
using PharmacyInventoryDispensingSystem.Application.Features.Patients.Mappers;
using PharmacyInventoryDispensingSystem.Domain.Common.Results;
using System;
using System.Collections.Generic;
using System.Text;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace PharmacyInventoryDispensingSystem.Application.Features.Patients.Queries.GetPatientById
{
    public sealed class GetPatientByIdQueryHandler(IPatientRepository patientRepository
        ,ILogger<GetPatientByIdQueryHandler> logger)
        : IRequestHandler<GetPatientByIdQuery, Result<PatientResponseDto>>
    {
        public async Task<Result<PatientResponseDto>> Handle(
            GetPatientByIdQuery query,
            CancellationToken cancellationToken)
        {
            var patient= await patientRepository.GetByIdAsync(
                query.PatientId,
                cancellationToken: cancellationToken);

            if (patient is null) 
            {
                logger.LogWarning(
                    "Patient with id {PatientId} was not found",
                    query.PatientId);

                return PatientErrors.NotFound(query.PatientId);

            }

            //Map to dto

            return patient.ToDto();

        }
    }
}
