using MediatR;
using Microsoft.Extensions.Logging;
using PharmacyInventoryDispensingSystem.Application.Common.Errors;
using PharmacyInventoryDispensingSystem.Application.Common.Interfaces;
using PharmacyInventoryDispensingSystem.Application.Common.Interfaces.Repositories;
using PharmacyInventoryDispensingSystem.Application.Features.Prescriptions.Dtos;
using PharmacyInventoryDispensingSystem.Domain.Common.Results;
using PharmacyInventoryDispensingSystem.Domain.Entities.Medicines;
using PharmacyInventoryDispensingSystem.Domain.Entities.Prescriptions;
using PharmacyInventoryDispensingSystem.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace PharmacyInventoryDispensingSystem.Application.Features.Prescriptions.Commands.CreatePrescription
{
    public sealed class CreatePrescriptionCommandHandler(
        IPrescriptionRepository prescriptionRepository,
        IPatientRepository patientRepository,
        IMedicineRepository medicineRepository,
        ICurrentUser currentUser,
        IUnitOfWork unitOfWork,
        ILogger<CreatePrescriptionCommandHandler> logger)

       : IRequestHandler<CreatePrescriptionCommand, Result<CreatePrescriptionResponse>>

    {
        public async Task<Result<CreatePrescriptionResponse>> Handle(CreatePrescriptionCommand request, CancellationToken cancellationToken)
        {
            //→Verify Patient exists 

            var patient = await patientRepository.GetByIdAsync(request.PatientId,
                                                              trackChanges: false,
                                                              cancellationToken);

            if (patient is null)
            {
                logger.LogWarning("Patient with ID {PatientId} not found.", request.PatientId);

                return PatientErrors.NotFound(request.PatientId);
            }


            //→ 2.Load all requested medicines in a single database query to avoid N queries:


            var medicineIds = request.Items
                            .Select(item => item.MedicineId)
                            .ToList();

            var medicines = await medicineRepository
                            .GetByIdsAsync(medicineIds, cancellationToken);

            //→ Convert the result to a dictionary for O(1) lookup by MedicineId:
            var medicinesById = medicines
                                .ToDictionary(medicine => medicine.Id);

            //→  then validate that each requested medicine exists and is active:

            foreach (var medicineId in medicineIds)
            {
                if (!medicinesById.TryGetValue(medicineId, out var medicine))
                {
                    logger.LogWarning("Medicine with ID {MedicineId} not found.", medicineId);
                    return MedicineErrors.NotFound(medicineId);
                }


                if (!medicine.IsActive)
                {
                    logger.LogWarning("Medicine with ID {MedicineId} is inactive.", medicineId);
                    return MedicineErrors.Inactive(medicine.Code);
                }
            }

            //→ Generate PrescriptionNumber:

            var prescriptionNumber = await prescriptionRepository
                                    .GenerateNextPrescriptionNumberAsync(cancellationToken);



            var prescription = new Prescription
            {
                PrescriptionNumber = prescriptionNumber,
                PatientId = request.PatientId,
                DoctorId = currentUser.Id!,
                ValidFrom = request.ValidFrom,
                ValidTo = request.ValidTo,
                Status = PrescriptionStatus.Active,
                Notes = request.Notes,

            };

            //→ Create PrescriptionItems and associate them with the Prescription:

            foreach (var item in request.Items)
            {
                prescription.Items.Add(new PrescriptionItem
                {
                    MedicineId = item.MedicineId,
                    QuantityPrescribed = item.QuantityPrescribed,
                    MaxFillCount = item.MaxFillCount,
                    FillUsedCount = 0,
                    DosageInstructions = item.DosageInstructions
                });

            }

            await prescriptionRepository.AddAsync(
           prescription,
           cancellationToken);

            await unitOfWork.SaveChangesAsync(cancellationToken);

            logger.LogInformation(
                "Prescription {PrescriptionNumber} created successfully for patient {PatientId}.",
                prescription.PrescriptionNumber,
                prescription.PatientId);

            return new CreatePrescriptionResponse(
                prescription.Id,
                prescription.PrescriptionNumber);
        }
    }
}
