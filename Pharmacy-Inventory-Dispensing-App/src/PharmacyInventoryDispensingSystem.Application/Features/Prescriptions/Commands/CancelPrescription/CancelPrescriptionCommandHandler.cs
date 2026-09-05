using MediatR;
using Microsoft.Extensions.Logging;
using PharmacyInventoryDispensingSystem.Application.Common.Interfaces.Authorization;
using PharmacyInventoryDispensingSystem.Application.Common.Interfaces.Repositories;
using PharmacyInventoryDispensingSystem.Domain.Common.Results;
using PharmacyInventoryDispensingSystem.Domain.Entities.Prescriptions;
using PharmacyInventoryDispensingSystem.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace PharmacyInventoryDispensingSystem.Application.Features.Prescriptions.Commands.CancelPrescription
{
    public sealed class CancelPrescriptionCommandHandler(
    IPrescriptionRepository prescriptionRepository,
    IPrescriptionAuthorizationService prescriptionAuthorizationService,
    IUnitOfWork unitOfWork,
    ILogger<CancelPrescriptionCommandHandler> logger)
    : IRequestHandler<CancelPrescriptionCommand, Result<Updated>>
    {
        public async Task<Result<Updated>> Handle(
            CancelPrescriptionCommand request,
            CancellationToken cancellationToken)
        {
            var prescription=await prescriptionRepository.GetByIdForCancellationAsync(
                request.PrescriptionId,
                cancellationToken);

            if (prescription is null)
            {
                logger.LogWarning(
                    "Prescription with ID {PrescriptionId} was not found.",
                    request.PrescriptionId);

                return PrescriptionErrors.NotFound(request.PrescriptionId);
            }

            // Check if the user is authorized to cancel the prescription:

            bool canAccess=await prescriptionAuthorizationService.CanAccessAsync(
                prescription,
                cancellationToken);


            if (!canAccess)
            {
                logger.LogWarning(
                    "User is not permitted to cancel prescription {PrescriptionId}.",
                    prescription.Id);

                return PrescriptionErrors.Forbidden;
            }


            // Check if the prescription is already cancelled or expired:

            if (prescription.Status == PrescriptionStatus.Cancelled)
            {
                logger.LogWarning(
                    "Prescription {PrescriptionId} is already cancelled.",
                    prescription.Id);
                return PrescriptionErrors.AlreadyCancelled;
            }



            var todayDate = DateOnly.FromDateTime(DateTime.UtcNow);

            if (prescription.Status == PrescriptionStatus.Expired || 
                prescription.ValidTo < todayDate)
            {
                logger.LogWarning(
                    "Expired prescription {PrescriptionId} cannot be cancelled.",
                    prescription.Id);
                return PrescriptionErrors.CannotCancelExpired;
            }


            // Cancel the prescription:

            prescription.Status = PrescriptionStatus.Cancelled;

            await unitOfWork.SaveChangesAsync(cancellationToken);

            logger.LogInformation(
           "Prescription {PrescriptionId} was cancelled successfully.",
           prescription.Id);

            return Result.Updated;



        }
    }
}
