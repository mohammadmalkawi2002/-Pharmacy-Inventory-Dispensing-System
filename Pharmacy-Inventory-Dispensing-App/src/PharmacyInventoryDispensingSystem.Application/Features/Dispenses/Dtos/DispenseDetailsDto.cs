using System;
using System.Collections.Generic;
using System.Text;

namespace PharmacyInventoryDispensingSystem.Application.Features.Dispenses.Dtos
{
    /// <summary>
    /// Represents the complete details of a dispensing event,
    /// including prescription, patient, pharmacist, and dispensed items.
    /// </summary>
    public sealed record DispenseDetailsDto(
        Guid Id,
        Guid PrescriptionId,
        string PrescriptionNumber,
        Guid PatientId,
        string PatientName,
        string PatientDocumentId,
        string PharmacistId,
        string PharmacistName,
        DateTimeOffset DispensedAt,
        string? Notes,
        IReadOnlyCollection<DispenseItemDto> Items);
}
