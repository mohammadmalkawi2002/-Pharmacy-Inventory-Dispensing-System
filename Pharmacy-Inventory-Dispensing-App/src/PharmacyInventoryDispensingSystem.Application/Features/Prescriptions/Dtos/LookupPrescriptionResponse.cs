using PharmacyInventoryDispensingSystem.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace PharmacyInventoryDispensingSystem.Application.Features.Prescriptions.Dtos
{
    public sealed record LookupPrescriptionResponse(
    Guid PrescriptionId,
    string PrescriptionNumber,
    string PatientName,
    string PatientDocumentId,
    string DoctorName,
    DateOnly ValidFrom,
    DateOnly ValidTo,
    PrescriptionStatus Status,
    string? Notes,
    bool CanDispense,
    string? UnavailableReason,
    IReadOnlyCollection<LookupPrescriptionItemDto> Items);





    public sealed record LookupPrescriptionItemDto(
    Guid PrescriptionItemId,
    Guid MedicineId,
    string MedicineCode,
    string MedicineName,
    string Strength,
    MedicineForm Form,
    StockUnit StockUnit,
    int QuantityPrescribed,
    int QuantityInStock,
    int MaxFillCount,
    int FillUsedCount,
    int RemainingFillCount,
    string? DosageInstructions,
    bool CanDispense,
    string? UnavailableReason);
}


