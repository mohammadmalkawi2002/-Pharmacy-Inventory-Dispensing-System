using MediatR;
using Microsoft.Extensions.Logging;
using PharmacyInventoryDispensingSystem.Application.Common.Errors;
using PharmacyInventoryDispensingSystem.Application.Common.Interfaces.Repositories;
using PharmacyInventoryDispensingSystem.Domain.Common.Results;
using System;
using System.Collections.Generic;
using System.Text;

namespace PharmacyInventoryDispensingSystem.Application.Features.Patients.Commands.RestorePatient
{
    public sealed class RestorePatientCommandHandler(
        IPatientRepository patientRepository,
        IUnitOfWork unitOfWork,
        ILogger<RestorePatientCommandHandler> logger) : IRequestHandler<RestorePatientCommand, Result<Updated>>



    {
        public async Task<Result<Updated>> Handle(
            RestorePatientCommand request,
            CancellationToken cancellationToken)
        {

            var patient=await patientRepository.GetByIdIncludingArchivedAsync(request.PatientId,cancellationToken);

            if (patient is null) 
            {
                logger.LogWarning(" Patient {PatientId} was not found. Restore was rejected.", request.PatientId);

                return PatientErrors.NotFound(request.PatientId);
            
            }


            if (!patient.IsDeleted) 
            {
                logger.LogWarning("Patient {PatientId} is not archived. Restore was rejected.", request.PatientId);


                return PatientErrors.NotArchived(request.PatientId);

            }

            patient.Restore();

            await unitOfWork.SaveChangesAsync(cancellationToken);

            logger.LogInformation("Patient {PatientId} was restored successfully.",request.PatientId);

            return Result.Updated;

        }
    }
}
