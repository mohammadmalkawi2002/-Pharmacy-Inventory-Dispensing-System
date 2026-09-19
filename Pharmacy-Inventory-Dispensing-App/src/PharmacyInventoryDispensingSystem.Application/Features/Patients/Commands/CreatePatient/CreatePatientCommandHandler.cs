using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using PharmacyInventoryDispensingSystem.Application.Common.Errors;
using PharmacyInventoryDispensingSystem.Application.Common.Interfaces.Repositories;
using PharmacyInventoryDispensingSystem.Application.Features.Patients.Dtos;
using PharmacyInventoryDispensingSystem.Application.Features.Patients.Mappers;
using PharmacyInventoryDispensingSystem.Domain.Common.Results;
using PharmacyInventoryDispensingSystem.Domain.Entities.Patients;
using System;
using System.Collections.Generic;
using System.Text;

namespace PharmacyInventoryDispensingSystem.Application.Features.Patients.Commands.CreatePatient
{
    public sealed class CreatePatientCommandHandler(
      IPatientRepository patientRepository,
        IUnitOfWork unitOfWork,
        ILogger<CreatePatientCommandHandler> logger)
        : IRequestHandler<CreatePatientCommand, Result<PatientResponseDto>>
    {
        public async Task<Result<PatientResponseDto>> Handle(CreatePatientCommand request, CancellationToken cancellationToken)
        {

          var documentId=request.DocumentId.Trim();

            //→ Check DocumentId uniqueness (including archived):
            var documentIdExists = await patientRepository.ExistsByDocumentIdAsync(documentId, cancellationToken);

            if (documentIdExists) 
            {
                logger.LogWarning("Patient creation aborted. DocumentId already exists. ");
                return PatientErrors.DocumentIdConflict;
            }

            //→ Create Patient:
            var patient = new Patient
            {
                DocumentId = documentId,
                FullName = request.FullName.Trim(),
                DateOfBirth = request.DateOfBirth.Date,
                PhoneNumber = request.PhoneNumber.Trim(),
                

            };

            patientRepository.Add(patient);

            await unitOfWork.SaveChangesAsync(cancellationToken);

            logger.LogInformation(
                "Patient {PatientId} was created successfully",
                patient.Id);

            return patient.ToDto();
        }
    }
}
