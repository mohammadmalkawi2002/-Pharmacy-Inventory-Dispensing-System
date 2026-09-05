using MediatR;
using Microsoft.Extensions.Logging;
using PharmacyInventoryDispensingSystem.Application.Common.Interfaces.Authorization;
using PharmacyInventoryDispensingSystem.Application.Common.Interfaces.Repositories;
using PharmacyInventoryDispensingSystem.Domain.Common.Results;
using PharmacyInventoryDispensingSystem.Domain.Entities.Medicines;
using PharmacyInventoryDispensingSystem.Domain.Entities.Prescriptions;
using PharmacyInventoryDispensingSystem.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace PharmacyInventoryDispensingSystem.Application.Features.Prescriptions.Commands.UpdatePrescription
{
    public sealed class UpdatePrescriptionCommandHandler(
        IPrescriptionRepository prescriptionRepository,
        IMedicineRepository medicineRepository,
        IDispenseRepository dispenseRepository,
        IPrescriptionAuthorizationService authorizationService,
        IUnitOfWork unitOfWork,
        ILogger<UpdatePrescriptionCommandHandler> logger)
        : IRequestHandler<UpdatePrescriptionCommand, Result<Updated>>
    {
        public async Task<Result<Updated>> Handle(
            UpdatePrescriptionCommand request,
            CancellationToken cancellationToken)
        {
            // 1. Get tracked prescription with items
            var prescription = await prescriptionRepository.GetByIdAsync(
                request.PrescriptionId,
                cancellationToken);

            if (prescription is null)
            {
                logger.LogWarning(
                    "Prescription with ID {PrescriptionId} not found.",
                    request.PrescriptionId);

                return PrescriptionErrors.NotFound(request.PrescriptionId);
            }

            // 2. Resource-based authorization
            // Doctor can update only their own prescriptions.
            // Admin can bypass ownership.
            bool canAccess = await authorizationService.CanAccessAsync(
                prescription,
                cancellationToken);

            if (!canAccess)
            {
                logger.LogWarning(
                    "User attempted unauthorized update of prescription {PrescriptionId}.",
                    prescription.Id);

                return PrescriptionErrors.Forbidden;
            }

            // 3. Only Active prescriptions can be updated
            if (prescription.Status != PrescriptionStatus.Active)
            {
                logger.LogWarning(
                    "InActive prescription {PrescriptionId} cannot be updated.",
                    prescription.Id);

                return PrescriptionErrors.CannotUpdateInActive;
            }

            

            // 4. Once dispensing has started, the prescription is immutable
            bool hasDispensingHistory =
                await dispenseRepository.ExistsForPrescriptionAsync(
                    prescription.Id,
                    cancellationToken);

            if (hasDispensingHistory)
            {
                logger.LogWarning(
                    "Prescription {PrescriptionId} cannot be updated because it has dispensing history.",
                    prescription.Id);

                return PrescriptionErrors.CannotUpdateDispensed;
            }

            // 5. Load all requested medicines in one query
            var medicineIds = request.Items
                .Select(item => item.MedicineId)
                .ToList();

            var medicines = await medicineRepository.GetByIdsAsync(
                medicineIds,
                cancellationToken);

            // 6. Ensure all requested medicines exist and are not archived
            if (medicines.Count != medicineIds.Count)
            {
                var foundMedicineIds = medicines
                    .Select(medicine => medicine.Id)
                    .ToHashSet();

                var missingMedicineId = medicineIds
                    .First(id => !foundMedicineIds.Contains(id));

                logger.LogWarning(
                    "Medicine with ID {MedicineId} was not found while updating prescription {PrescriptionId}.",
                    missingMedicineId,
                    prescription.Id);

                return MedicineErrors.NotFound(missingMedicineId);
            }

            // 7. Ensure all requested medicines are active
            var inactiveMedicine = medicines
                .FirstOrDefault(medicine => !medicine.IsActive);

            if (inactiveMedicine is not null)
            {
                logger.LogWarning(
                    "Inactive medicine {MedicineId} cannot be added to prescription {PrescriptionId}.",
                    inactiveMedicine.Id,
                    prescription.Id);

                return MedicineErrors.Inactive(inactiveMedicine.Code);
            }

            // 8. Update prescription fields
            prescription.ValidFrom = request.ValidFrom;
            prescription.ValidTo = request.ValidTo;
            prescription.Notes = request.Notes?.Trim();

            // 9. Prepare lookup for requested items
            var requestedItemsByMedicineId = request.Items
                .ToDictionary(item => item.MedicineId);

            var existingItemsByMedicineId = prescription.Items
                .ToDictionary(item => item.MedicineId);

            // 10. Update existing items
            foreach (var existingItem in prescription.Items)
            {
                if (!requestedItemsByMedicineId.TryGetValue(
                        existingItem.MedicineId,
                        out var requestedItem))
                {
                    continue;
                }

                existingItem.QuantityPrescribed =
                    requestedItem.QuantityPrescribed;

                existingItem.MaxFillCount =
                    requestedItem.MaxFillCount;

                existingItem.DosageInstructions =
                    requestedItem.DosageInstructions?.Trim();

                // FillUsedCount is intentionally NOT modified.
            }

            // 11. Remove items that are no longer in the request
            var itemsToRemove = prescription.Items
                .Where(existingItem =>
                    !requestedItemsByMedicineId.ContainsKey(
                        existingItem.MedicineId))
                .ToList();

            foreach (var item in itemsToRemove)
            {
                prescriptionRepository.RemoveItem(item);
            }

            // 12. Add newly requested items
            foreach (var requestedItem in request.Items)
            {
                if (existingItemsByMedicineId.ContainsKey(
                        requestedItem.MedicineId))
                {
                    continue;
                }

                prescription.Items.Add(
                    new PrescriptionItem
                    {
                        MedicineId = requestedItem.MedicineId,
                        QuantityPrescribed = requestedItem.QuantityPrescribed,
                        MaxFillCount = requestedItem.MaxFillCount,
                        FillUsedCount = 0,
                        DosageInstructions =
                            requestedItem.DosageInstructions?.Trim()
                    });
            }

            // 13. Persist everything in one transaction/unit of work
            await unitOfWork.SaveChangesAsync(cancellationToken);

            return Result.Updated ;
        }
    }
}
