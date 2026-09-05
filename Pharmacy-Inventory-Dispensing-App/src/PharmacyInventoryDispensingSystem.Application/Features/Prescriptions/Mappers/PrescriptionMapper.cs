using PharmacyInventoryDispensingSystem.Application.Features.Prescriptions.Dtos;
using PharmacyInventoryDispensingSystem.Domain.Entities.Prescriptions;
using PharmacyInventoryDispensingSystem.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace PharmacyInventoryDispensingSystem.Application.Features.Prescriptions.Mappers
{
    public static class PrescriptionMapper
    {
        public static PrescriptionDetailsDto ToDetailsDto(this Prescription prescription, string doctorName)
        {

            return new PrescriptionDetailsDto(
                prescription.Id,
                prescription.PrescriptionNumber,
                prescription.PatientId,
                prescription.Patient.DocumentId,
                prescription.Patient.FullName,
                doctorName,
                prescription.ValidFrom,
                prescription.ValidTo,
                prescription.Status,
                prescription.Notes,
                prescription.CreatedAtUtc,
                prescription.Items
                    .Select(item => new PrescriptionItemDto(
                        item.MedicineId,
                        item.Medicine.Code,
                        item.Medicine.Name,
                        item.Medicine.Strength,
                        item.Medicine.Form,
                        item.Medicine.StockUnit,
                        item.QuantityPrescribed,
                        item.MaxFillCount,
                        item.FillUsedCount,
                        item.RemainingFillCount,
                        item.DosageInstructions))
                    .ToList());
        }


        public static PrescriptionSummaryDto ToSummaryDto(
            this Prescription entity,
            string doctorName)
        {
            ArgumentNullException.ThrowIfNull(entity);
            ArgumentException.ThrowIfNullOrWhiteSpace(doctorName);

            return new PrescriptionSummaryDto(
                Id: entity.Id,
                PrescriptionNumber: entity.PrescriptionNumber,
                PatientId: entity.PatientId,
                PatientName: entity.Patient.FullName,
                DoctorName: doctorName,
                ValidFrom: entity.ValidFrom,
                ValidTo: entity.ValidTo,
                Status: entity.Status,
                CreatedAtUtc: entity.CreatedAtUtc);
        }



        public static List<PrescriptionSummaryDto> ToSummaryDtos(
            this IEnumerable<Prescription> entities,
            IReadOnlyDictionary<string, string> doctorNames)
        {
            ArgumentNullException.ThrowIfNull(entities);
            ArgumentNullException.ThrowIfNull(doctorNames);

            return
            [
                .. entities.Select(entity =>
        {
            doctorNames.TryGetValue(
                entity.DoctorId,
                out var doctorName);

            return entity.ToSummaryDto(
                doctorName ?? "Unknown Doctor");
        })
            ];
        }



        public static LookupPrescriptionResponse ToLookupDto(
            this Prescription entity,
            string doctorName)
        {
            ArgumentNullException.ThrowIfNull(entity);
            ArgumentException.ThrowIfNullOrWhiteSpace(doctorName);

            var today = DateOnly.FromDateTime(DateTime.UtcNow);

            var prescriptionUnavailableReason =
                GetPrescriptionUnavailableReason(entity, today);

            bool prescriptionCanDispense =
                prescriptionUnavailableReason is null;

            var items = entity.Items
                .Select(item =>
                {
                    var itemUnavailableReason = prescriptionCanDispense
                        ? GetItemUnavailableReason(item)
                        : prescriptionUnavailableReason;

                    return new LookupPrescriptionItemDto(
                        PrescriptionItemId: item.Id,
                        MedicineId: item.MedicineId,
                        MedicineCode: item.Medicine.Code,
                        MedicineName: item.Medicine.Name,
                        Strength: item.Medicine.Strength,
                        Form: item.Medicine.Form,
                        StockUnit: item.Medicine.StockUnit,
                        QuantityPrescribed: item.QuantityPrescribed,
                        QuantityInStock: item.Medicine.QuantityInStock,
                        MaxFillCount: item.MaxFillCount,
                        FillUsedCount: item.FillUsedCount,
                        RemainingFillCount: item.RemainingFillCount,
                        DosageInstructions: item.DosageInstructions,
                        CanDispense: itemUnavailableReason is null,
                        UnavailableReason: itemUnavailableReason);
                })
                .ToList();

            return new LookupPrescriptionResponse(
                PrescriptionId: entity.Id,
                PrescriptionNumber: entity.PrescriptionNumber,
                PatientName: entity.Patient.FullName,
                PatientDocumentId: entity.Patient.DocumentId,
                DoctorName: doctorName,
                ValidFrom: entity.ValidFrom,
                ValidTo: entity.ValidTo,
                Status: entity.Status,
                Notes: entity.Notes,
                CanDispense: prescriptionCanDispense,
                UnavailableReason: prescriptionUnavailableReason,
                Items: items);
        }


        //Prescription-level helper(status reason for entire prescription )
        private static string? GetPrescriptionUnavailableReason(Prescription prescription, DateOnly today)
        {
            if (prescription.Status == PrescriptionStatus.Cancelled)
                return "Prescription is cancelled.";

            if (prescription.Status == PrescriptionStatus.Expired ||
                prescription.ValidTo < today)
                return "Prescription has expired.";

            if (prescription.ValidFrom > today)
                return "Prescription is not valid yet.";

            return null;
        }

        //Item-level helper
        private static string? GetItemUnavailableReason(PrescriptionItem item)

        {
            if (!item.Medicine.IsActive)
                return "Medicine is inactive.";

            if (!item.HasFillsRemaining)
                return "No fills remaining.";

            if (item.Medicine.QuantityInStock < item.QuantityPrescribed)
                return "Insufficient stock.";

            return null;
        }


    }
}
