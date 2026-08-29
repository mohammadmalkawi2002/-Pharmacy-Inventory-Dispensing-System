using MediatR;
using Microsoft.Extensions.Logging;
using PharmacyInventoryDispensingSystem.Application.Common.Errors;
using PharmacyInventoryDispensingSystem.Application.Common.Interfaces.Repositories;
using PharmacyInventoryDispensingSystem.Application.Features.Patients.Dtos;
using PharmacyInventoryDispensingSystem.Domain.Common.Results;
using System;
using System.Collections.Generic;
using System.Text;

namespace PharmacyInventoryDispensingSystem.Application.Features.Patients.Commands.UpdatePatient
{
    public sealed class UpdatePatientCommandHandler(
        IPatientRepository patientRepository,
        IUnitOfWork unitOfWork,
        ILogger<UpdatePatientCommandHandler> logger)
      : IRequestHandler<
          UpdatePatientCommand,
          Result<Updated>>
    {
        public async Task<Result<Updated>> Handle(UpdatePatientCommand request, CancellationToken cancellationToken)
        {
            var patient=await patientRepository.GetByIdAsync(
                request.PatientId,
                trackChanges: true,
                cancellationToken);

            if (patient is null) 
            {
                logger.LogWarning(
                    "Patient {PatientId} not found the update rejected .",
                    request.PatientId);

                return PatientErrors.NotFound(request.PatientId);
          

            }


            var documentId = request.DocumentId.Trim();

            bool documentIdChanged=
                patient.DocumentId != documentId;

            if (documentIdChanged)
            {
                var documentIdExists = await patientRepository.ExistsByDocumentIdAsync(
                    documentId,
                    cancellationToken);


                if (documentIdExists)
                {
                    logger.LogWarning(
                        "Patient update was rejected for {PatientId} because A patient with the same document ID already exists",
                        patient.Id);
                        
                    return PatientErrors.DocumentIdConflict;
                }
            }



            patient.DocumentId = documentId;
            patient.FullName=request.FullName.Trim();
            patient.DateOfBirth = request.DateOfBirth.Date;
            patient.PhoneNumber = request.PhoneNumber.Trim();

            await unitOfWork.SaveChangesAsync(cancellationToken);

            logger.LogInformation(
                "Patient {PatientId} was updated successfully",
                patient.Id);

            return Result.Updated;
        }
    }
}
