using MediatR;
using PharmacyInventoryDispensingSystem.Application.Common.Interfaces;
using PharmacyInventoryDispensingSystem.Application.Common.Interfaces.Repositories;
using PharmacyInventoryDispensingSystem.Application.Features.Dispenses.Dtos;
using PharmacyInventoryDispensingSystem.Domain.Common.Results;
using PharmacyInventoryDispensingSystem.Domain.Entities.Dispenses;
using System;
using System.Collections.Generic;
using System.Text;

namespace PharmacyInventoryDispensingSystem.Application.Features.Dispenses.Queries.GetDispenseById
{
    public sealed class GetDispenseByIdQueryHandler(
    IDispenseRepository dispenseRepository,
    IUserLookupService userLookupService)
    : IRequestHandler<GetDispenseByIdQuery, Result<DispenseDetailsDto>>
    {
        public async Task<Result<DispenseDetailsDto>> Handle(
            GetDispenseByIdQuery query,
            CancellationToken cancellationToken)
        {
            // Load the dispense record with its prescription,
            // patient, dispensed items, and medicines.
            Dispense? dispense =
                await dispenseRepository.GetByIdWithDetailsAsync(
                    query.DispenseId,
                    cancellationToken);

            if (dispense is null)
            {
                return DispenseErrors.DispenseNotFound;
            }

            // Resolve the name of the user who performed the dispensing.
            IReadOnlyDictionary<string, string> userNames =
                await userLookupService.GetUserNamesByIdsAsync(
                    [dispense.PharmacistId],
                    cancellationToken);

            string dispensedByName = userNames.TryGetValue(
                dispense.PharmacistId,
                out string? resolvedName)
                    ? resolvedName
                    : dispense.PharmacistId;

            var response = new DispenseDetailsDto(
                dispense.Id,
                dispense.PrescriptionId,
                dispense.Prescription.PrescriptionNumber,
                dispense.Prescription.PatientId,
                dispense.Prescription.Patient.FullName,
                dispense.Prescription.Patient.DocumentId,
                dispense.PharmacistId,
                dispensedByName,
                dispense.DispensedAt,
                dispense.Notes,
                dispense.Items
                    .Select(item => new DispenseItemDto(
                        item.Id,
                        item.PrescriptionItemId,
                        item.PrescriptionItem.MedicineId,
                        item.PrescriptionItem.Medicine.Code,
                        item.PrescriptionItem.Medicine.Name,
                        item.PrescriptionItem.Medicine.Strength,
                        item.PrescriptionItem.Medicine.StockUnit,
                        item.Quantity,
                        item.PrescriptionItem.DosageInstructions))
                    .ToList());

            return response;
        }
    }
    }
