using PharmacyInventoryDispensingSystem.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace PharmacyInventoryDispensingSystem.Application.Features.Prescriptions.Dtos
{
    public sealed record PrescriptionDetailsDto(
        Guid Id,
        string PrescriptionNumber,
        Guid PatientId,
        string PatientDocumentId,
        string PatientName,
        string DoctorName,
        DateOnly ValidFrom,
        DateOnly ValidTo,
        PrescriptionStatus Status,
        string? Notes,
        DateTimeOffset CreatedAtUtc,
        IReadOnlyCollection<PrescriptionItemDto> Items);



    public sealed record PrescriptionItemDto(
        Guid MedicineId,
        string MedicineCode,
        string MedicineName,
        string Strength,
        MedicineForm Form,
        StockUnit StockUnit,
        int QuantityPrescribed,
        int MaxFillCount,
        int FillUsedCount,
        int RemainingFillCount,
        string? DosageInstructions);
}
