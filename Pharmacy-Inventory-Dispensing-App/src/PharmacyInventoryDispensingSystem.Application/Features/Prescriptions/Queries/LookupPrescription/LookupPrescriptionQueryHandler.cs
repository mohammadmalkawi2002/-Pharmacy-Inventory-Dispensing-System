using MediatR;
using Microsoft.Extensions.Logging;
using PharmacyInventoryDispensingSystem.Application.Common.Interfaces;
using PharmacyInventoryDispensingSystem.Application.Common.Interfaces.Repositories;
using PharmacyInventoryDispensingSystem.Application.Features.Prescriptions.Dtos;
using PharmacyInventoryDispensingSystem.Application.Features.Prescriptions.Mappers;
using PharmacyInventoryDispensingSystem.Domain.Common.Results;
using PharmacyInventoryDispensingSystem.Domain.Entities.Patients;
using PharmacyInventoryDispensingSystem.Domain.Entities.Prescriptions;
using System;
using System.Collections.Generic;
using System.Reflection.Metadata;
using System.Text;

namespace PharmacyInventoryDispensingSystem.Application.Features.Prescriptions.Queries.LookupPrescription
{
    public sealed class LookupPrescriptionQueryHandler(
        IPrescriptionRepository prescriptionRepository,
        IUserLookupService userLookupService,
        ILogger<LookupPrescriptionQueryHandler> logger)
      : IRequestHandler<
          LookupPrescriptionQuery,
          Result<LookupPrescriptionResponse>>
    {
        public async Task<Result<LookupPrescriptionResponse>> Handle(
            LookupPrescriptionQuery request,
            CancellationToken cancellationToken)
        {
            var prescription=await prescriptionRepository.LookupAsync(
                request.PrescriptionNumber.Trim(),
                request.DocumentId.Trim(),
                cancellationToken);

            if (prescription is null)
            {
                logger.LogWarning(
                    "Prescription lookup failed for prescription number {PrescriptionNumber}.",
                    request.PrescriptionNumber);

                return PrescriptionErrors.LookupNotFound;
            }

            // Load doctor names

            var doctorNames = await userLookupService.GetUserNamesByIdsAsync([prescription.DoctorId],
                                                         cancellationToken);


            doctorNames.TryGetValue(
                prescription.DoctorId,
                out var doctorName);


            return prescription.ToLookupDto(
         doctorName ?? "Unknown Doctor");

        }
    }
}


