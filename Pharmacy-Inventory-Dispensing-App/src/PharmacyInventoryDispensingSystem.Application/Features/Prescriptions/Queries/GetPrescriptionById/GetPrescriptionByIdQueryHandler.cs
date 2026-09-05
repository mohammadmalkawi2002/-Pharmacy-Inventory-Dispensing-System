using MediatR;
using Microsoft.Extensions.Logging;
using PharmacyInventoryDispensingSystem.Application.Common.Interfaces;
using PharmacyInventoryDispensingSystem.Application.Common.Interfaces.Authorization;
using PharmacyInventoryDispensingSystem.Application.Common.Interfaces.Repositories;
using PharmacyInventoryDispensingSystem.Application.Features.Prescriptions.Dtos;
using PharmacyInventoryDispensingSystem.Application.Features.Prescriptions.Mappers;
using PharmacyInventoryDispensingSystem.Domain.Common.Results;
using PharmacyInventoryDispensingSystem.Domain.Entities.Prescriptions;
using System;
using System.Collections.Generic;
using System.Text;

namespace PharmacyInventoryDispensingSystem.Application.Features.Prescriptions.Queries.GetPrescriptionById
{
    public sealed class GetPrescriptionByIdQueryHandler(
    IPrescriptionRepository prescriptionRepository,
    IPrescriptionAuthorizationService prescriptionAuthorizationService,
    IUserLookupService userLookupService,
    ILogger<GetPrescriptionByIdQueryHandler> logger)
    : IRequestHandler<GetPrescriptionByIdQuery, Result<PrescriptionDetailsDto>>
    {
        public async Task<Result<PrescriptionDetailsDto>> Handle(
            GetPrescriptionByIdQuery request,
            CancellationToken cancellationToken)
        {
            // Retrieve the prescription by ID with details(items medicine  and patient information) from the repository
            var prescription = await prescriptionRepository.GetByIdWithDetailsAsync(request.
                PrescriptionId, cancellationToken);

            if (prescription is null)
            {
                logger.LogWarning(
                    "Prescription with ID {PrescriptionId} not found.",
                    request.PrescriptionId);

                return PrescriptionErrors.NotFound(request.PrescriptionId);
            }

            //Ownership Authorization check:

            bool canAccess=await prescriptionAuthorizationService.CanAccessAsync(
                prescription,
                cancellationToken);


            if (!canAccess)
            {
                logger.LogWarning(
                    "User attempted unauthorized access to prescription {PrescriptionId}.",
                    prescription.Id);

                return PrescriptionErrors.Forbidden;
            }

            // Retrieve the doctor's name using the user lookup service
            var userNames = await userLookupService.GetUserNamesByIdsAsync(
            [prescription.DoctorId],
            cancellationToken);

            // Get the doctor's name from the dictionary, or use "Unknown Doctor" if not found
            userNames.TryGetValue(
                prescription.DoctorId,
                out var doctorName);
            // Map the prescription entity to a PrescriptionDetailsDto and return it
            return prescription.ToDetailsDto(
                    doctorName?? "Unknown Doctor");


        }
    }
}
