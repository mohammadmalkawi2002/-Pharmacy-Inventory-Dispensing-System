using PharmacyInventoryDispensingSystem.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace PharmacyInventoryDispensingSystem.Application.Features.Prescriptions.Dtos
{
    public sealed record PrescriptionSummaryDto(
        Guid Id,
        string PrescriptionNumber,
        Guid PatientId,
        string PatientName,
        string DoctorName,
        DateOnly ValidFrom,
        DateOnly ValidTo,
        PrescriptionStatus Status,
        DateTimeOffset CreatedAtUtc);
}
