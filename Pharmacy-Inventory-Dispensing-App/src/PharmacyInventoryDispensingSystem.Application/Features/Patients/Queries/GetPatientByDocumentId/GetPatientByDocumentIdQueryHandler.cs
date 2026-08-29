using MediatR;
using Microsoft.Extensions.Logging;
using PharmacyInventoryDispensingSystem.Application.Common.Errors;
using PharmacyInventoryDispensingSystem.Application.Common.Interfaces.Repositories;
using PharmacyInventoryDispensingSystem.Application.Features.Patients.Dtos;
using PharmacyInventoryDispensingSystem.Application.Features.Patients.Mappers;
using PharmacyInventoryDispensingSystem.Domain.Common.Results;
using PharmacyInventoryDispensingSystem.Domain.Entities.Patients;
using System;
using System.Collections.Generic;
using System.Reflection.Metadata;
using System.Text;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace PharmacyInventoryDispensingSystem.Application.Features.Patients.Queries.GetPatientByDocumentId
{
    public sealed class GetPatientByDocumentIdQueryHandler(IPatientRepository patientRepository,
        ILogger<GetPatientByDocumentIdQueryHandler> logger)
        : IRequestHandler<GetPatientByDocumentIdQuery, Result<PatientResponseDto>>
    {
        public async Task<Result<PatientResponseDto>> Handle(GetPatientByDocumentIdQuery request, CancellationToken cancellationToken)
        {
            var patient = await patientRepository.GetByDocumentIdAsync(
                request.DocumentId,
                cancellationToken);

            if (patient is null)
            {
                logger.LogWarning(
                    "Patient with the provided document ID {DocumentId} was not found",
                    request.DocumentId);

                return PatientErrors.NotFoundByDocumentId;

            }

           return patient.ToDto();
        }
    }
}
