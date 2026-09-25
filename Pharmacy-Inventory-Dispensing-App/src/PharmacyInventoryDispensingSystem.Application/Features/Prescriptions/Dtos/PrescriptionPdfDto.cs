using PharmacyInventoryDispensingSystem.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace PharmacyInventoryDispensingSystem.Application.Features.Prescriptions.Dtos
{
    public sealed record PrescriptionPdfDto(
     string PrescriptionNumber,
     string PatientDocumentId,
     string PatientName,
     int PatientAge,
     string DoctorName,
     DateOnly ValidFrom,
     DateOnly ValidTo,
     PrescriptionStatus Status,
     string? Notes,
     DateTimeOffset CreatedAtUtc,
     IReadOnlyCollection<PrescriptionPdfItemDto> Items);



    public sealed record PrescriptionPdfItemDto(
    string MedicineCode,
    string MedicineName,
    string Strength,
    MedicineForm Form,
    StockUnit StockUnit,
    int QuantityPrescribed,
    int MaxFillCount,
    string? DosageInstructions);
}
