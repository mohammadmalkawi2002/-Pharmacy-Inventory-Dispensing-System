using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using PharmacyInventoryDispensingSystem.Application.Common.Errors;
using PharmacyInventoryDispensingSystem.Application.Common.Interfaces;
using PharmacyInventoryDispensingSystem.Application.Common.Interfaces.Repositories;
using PharmacyInventoryDispensingSystem.Application.Features.Prescriptions.Dtos;
using PharmacyInventoryDispensingSystem.Domain.Common.Results;
using PharmacyInventoryDispensingSystem.Domain.Entities.Medicines;
using PharmacyInventoryDispensingSystem.Domain.Entities.Patients;
using PharmacyInventoryDispensingSystem.Domain.Entities.Prescriptions;
using PharmacyInventoryDispensingSystem.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace PharmacyInventoryDispensingSystem.Application.Features.Prescriptions.Commands.CreatePrescription
{
    public sealed class CreatePrescriptionCommandHandler(
         IPrescriptionRepository prescriptionRepository,
        IGenericRepository<Patient> patientRepository,
        IGenericRepository<Medicine> medicineRepository,
        ICurrentUser currentUser,
        IUnitOfWork unitOfWork,
        ILogger<CreatePrescriptionCommandHandler> logger)

       : IRequestHandler<CreatePrescriptionCommand, Result<CreatePrescriptionResponse>>

    {
        public async Task<Result<CreatePrescriptionResponse>> Handle(CreatePrescriptionCommand request, CancellationToken cancellationToken)
        {
            //→Verify Patient exists  :
            var patientExists = await patientRepository.Query()
                            .AnyAsync(p => p.Id == request.PatientId, cancellationToken);

            if (!patientExists)
            {
                logger.LogWarning("Patient with ID {PatientId} not found.", request.PatientId);
                return PatientErrors.NotFound(request.PatientId);
            }


            //→ 2.Load all requested medicines in a single database query to avoid N+1 queries:

            var medicineIds = request.Items
                            .Select(item => item.MedicineId)
                            .ToList();
            //  in db Contains convert like this : select ... WHERE Id IN(..,..)
            var medicines = await medicineRepository.Query()
                            .Where(m => medicineIds.Contains(m.Id))
                            .ToListAsync(cancellationToken);

            //→ 3.Convert the result to a dictionary for O(1) lookup by MedicineId:
            var medicinesById = medicines.ToDictionary(medicine => medicine.Id);


            //→ 4.then validate that each requested medicine exists and is active:

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

            //→ 5. Generate PrescriptionNumber:

            var prescriptionNumber = await prescriptionRepository
                                    .GenerateNextPrescriptionNumberAsync(cancellationToken);


            //→ 6.Create Prescription entity:
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

            //→7. Create PrescriptionItems and associate them with the Prescription:

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

            // → 8. Add and Save:
            prescriptionRepository.Add(prescription);

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
