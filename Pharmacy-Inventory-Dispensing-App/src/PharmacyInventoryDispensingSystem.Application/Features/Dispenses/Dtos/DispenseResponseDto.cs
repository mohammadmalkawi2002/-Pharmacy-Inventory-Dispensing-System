using System;
using System.Collections.Generic;
using System.Text;

namespace PharmacyInventoryDispensingSystem.Application.Features.Dispenses.Dtos
{/// <summary>
/// Represents a dispensing record displayed in the paginated dispensing history list.
/// </summary>
    public sealed record DispenseResponseDto(
    Guid Id,
    Guid PrescriptionId,
    string PrescriptionNumber,
    string PatientName,
    string PharmacistName,
    DateTimeOffset DispensedAt);
}
