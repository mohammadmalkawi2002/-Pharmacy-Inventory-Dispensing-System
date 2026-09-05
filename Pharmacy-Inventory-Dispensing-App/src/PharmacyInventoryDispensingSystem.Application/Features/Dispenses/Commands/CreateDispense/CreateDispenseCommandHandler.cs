using MediatR;
using Microsoft.Extensions.Logging;
using PharmacyInventoryDispensingSystem.Application.Common.Interfaces;
using PharmacyInventoryDispensingSystem.Application.Common.Interfaces.Repositories;
using PharmacyInventoryDispensingSystem.Application.Features.Dispenses.Dtos;
using PharmacyInventoryDispensingSystem.Domain.Common.Results;
using PharmacyInventoryDispensingSystem.Domain.Entities.Dispenses;
using PharmacyInventoryDispensingSystem.Domain.Entities.Prescriptions;
using PharmacyInventoryDispensingSystem.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace PharmacyInventoryDispensingSystem.Application.Features.Dispenses.Commands.CreateDispense
{
    public sealed class CreateDispenseCommandHandler(
     IPrescriptionRepository prescriptionRepository,
     IDispenseRepository dispenseRepository,
     IUnitOfWork unitOfWork,
     ICurrentUser currentUser,
     IUserLookupService userLookupService,
     ILogger<CreateDispenseCommandHandler> logger)
     : IRequestHandler<CreateDispenseCommand, Result<DispenseDetailsDto>>
    {
        public async Task<Result<DispenseDetailsDto>> Handle(
            CreateDispenseCommand command,
            CancellationToken cancellationToken)
        {

            // Step 1: Get the authenticated pharmacist ID or admin .
            string? pharmacistId = currentUser.Id;

            if (string.IsNullOrWhiteSpace(pharmacistId))
            {
                return Error.Unauthorized(
                    "Dispense.Unauthorized",
                    "The authenticated user could not be identified.");
            }

            DateTimeOffset dispensedAt = DateTimeOffset.UtcNow;
            DateOnly today = DateOnly.FromDateTime(dispensedAt.UtcDateTime);

            // Step 2: Load the prescription with its patient, items, and medicines.
            Prescription? prescription =
                await prescriptionRepository.GetForDispensingAsync(
                    command.PrescriptionId,
                    command.DocumentId.Trim(),
                    cancellationToken);

            if (prescription is null)
            {
                logger.LogWarning(
                    "Prescription {PrescriptionId} was not found or did not match the patient document ID",
                    command.PrescriptionId);

                return DispenseErrors.PrescriptionNotFound;
            }

            // Step 3: Validate the prescription status and validity period.
            Result<Success> prescriptionValidation =
                ValidatePrescriptionStatus(prescription, today);

            if (prescriptionValidation.IsError)
            {
                return prescriptionValidation.TopError;
            }

            // Step 4: Find only the items selected by the pharmacist.
            HashSet<Guid> selectedItemIds =
                command.PrescriptionItemIds.ToHashSet();

            List<PrescriptionItem> selectedItems = prescription.Items
                .Where(item => selectedItemIds.Contains(item.Id))
                .ToList();

            // Every selected item must exist and belong to this prescription.
            if (selectedItems.Count != selectedItemIds.Count)
            {
                return DispenseErrors.PrescriptionItemNotFound;
            }

            // Step 5: Validate all selected items before modifying any entity.
            foreach (PrescriptionItem item in selectedItems)
            {
                // Archived medicines are excluded by the global query filter.
                if (!item.Medicine.IsActive)
                {
                    return DispenseErrors.MedicineUnavailable(
                        item.Medicine.Name);
                }

                if (!item.HasFillsRemaining)
                {
                    return DispenseErrors.NoFillsRemaining(
                        item.Medicine.Name);
                }

                // The complete prescribed quantity must be available.
                Result<Success> stockAvailability =
                    item.Medicine.EnsureStockAvailable(
                        item.QuantityPrescribed);

                if (stockAvailability.IsError)
                {
                    return stockAvailability.TopError;
                }
            }

            // Step 6: Resolve the pharmacist name for the response.
            IReadOnlyDictionary<string, string> pharmacistNames =
                await userLookupService.GetUserNamesByIdsAsync(
                    [pharmacistId],
                    cancellationToken);

            string pharmacistName = pharmacistNames.TryGetValue(
                pharmacistId,
                out string? resolvedName)
                    ? resolvedName
                    : pharmacistId;

            // Step 7: Create the dispensing event.
            var dispense = new Dispense
            {
                PrescriptionId = prescription.Id,
                Prescription = prescription,
                PharmacistId = pharmacistId,
                DispensedAt = dispensedAt,
                Notes = string.IsNullOrWhiteSpace(command.Notes)
                    ? null
                    : command.Notes.Trim()
            };

            // Step 8: Decrease stock, increment fill usage,
            // and create a DispenseItem for every selected item.
            foreach (PrescriptionItem item in selectedItems)
            {
                Result<Success> decreaseStockResult =
                    item.Medicine.DecreaseStock(
                        item.QuantityPrescribed);

                if (decreaseStockResult.IsError)
                {
                    return decreaseStockResult.TopError;
                }

                item.FillUsedCount++;

                dispense.Items.Add(new DispenseItem
                {
                    PrescriptionItemId = item.Id,
                    PrescriptionItem = item,
                    Quantity = item.QuantityPrescribed
                });
            }

            // Step 9: Persist the dispense record, stock changes,
            // and fill-count changes atomically.
            await dispenseRepository.AddAsync(
                dispense,
                cancellationToken);

            await unitOfWork.SaveChangesAsync(cancellationToken);

            logger.LogInformation(
                "Dispense {DispenseId} was created for prescription {PrescriptionId} by pharmacist {PharmacistId}",
                dispense.Id,
                prescription.Id,
                pharmacistId);

            // Step 10: Build the response for the confirmation screen.
            var response = new DispenseDetailsDto(
                dispense.Id,
                prescription.Id,
                prescription.PrescriptionNumber,
                prescription.PatientId,
                prescription.Patient.FullName,
                prescription.Patient.DocumentId,
                pharmacistId,
                pharmacistName,
                dispense.DispensedAt,
                dispense.Notes,
                dispense.Items
                    .Select(dispenseItem => new DispenseItemDto(
                        dispenseItem.Id,
                        dispenseItem.PrescriptionItemId,
                        dispenseItem.PrescriptionItem.MedicineId,
                        dispenseItem.PrescriptionItem.Medicine.Code,
                        dispenseItem.PrescriptionItem.Medicine.Name,
                        dispenseItem.PrescriptionItem.Medicine.Strength,
                        dispenseItem.PrescriptionItem.Medicine.StockUnit,
                        dispenseItem.Quantity,
                        dispenseItem.PrescriptionItem.DosageInstructions))
                    .ToList());

            return response;
        }



        private static Result<Success> ValidatePrescriptionStatus(
            Prescription prescription,
            DateOnly today)
        {
            if (prescription.Status == PrescriptionStatus.Cancelled)
            {
                return DispenseErrors.PrescriptionCancelled;
            }

            if (prescription.Status == PrescriptionStatus.Expired ||
                today > prescription.ValidTo)
            {
                return DispenseErrors.PrescriptionExpired;
            }

            if (today < prescription.ValidFrom)
            {
                return DispenseErrors.PrescriptionNotYetValid;
            }

            return Result.Success;
        }
    }
    }
