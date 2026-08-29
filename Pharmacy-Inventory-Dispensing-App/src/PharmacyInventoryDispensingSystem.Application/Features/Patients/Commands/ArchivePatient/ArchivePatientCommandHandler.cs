using MediatR;
using MediatR.Pipeline;
using Microsoft.Extensions.Logging;
using PharmacyInventoryDispensingSystem.Application.Common.Errors;
using PharmacyInventoryDispensingSystem.Application.Common.Interfaces.Repositories;
using PharmacyInventoryDispensingSystem.Domain.Common.Results;
using System;
using System.Collections.Generic;
using System.Text;

namespace PharmacyInventoryDispensingSystem.Application.Features.Patients.Commands.ArchivePatient
{
    public sealed class ArchivePatientCommandHandler(
        IPatientRepository patientRepository,
        ILogger<ArchivePatientCommandHandler> logger, IUnitOfWork unitOfWork)
        : IRequestHandler<ArchivePatientCommand, Result<Deleted>>
    {
        public async Task<Result<Deleted>> Handle(ArchivePatientCommand request, CancellationToken cancellationToken)
        {
            // Get the patient Active|| Archived:
            var patient = await patientRepository.GetByIdIncludingArchivedAsync(
                request.PatientId,
                cancellationToken);

            if (patient is null) 
             {
                logger.LogWarning(
                   "Patient {PatientId} was not found. Archive was rejected.",
                   request.PatientId);

                return PatientErrors.NotFound(request.PatientId);            
            }


            // check already deleted:

            if (patient.IsDeleted) 
            {
                logger.LogWarning(
                   "Patient {PatientId} is already Archived.",
                   request.PatientId);

                return PatientErrors.AlreadyArchived(request.PatientId);
            }

            patient.Delete();

            await unitOfWork.SaveChangesAsync(cancellationToken);


            logger.LogInformation(
                "Patient {PatientId} was archived successfully.",
                request.PatientId);

            return Result.Deleted;

        }
    }
}
