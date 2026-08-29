using PharmacyInventoryDispensingSystem.Application.Features.Patients.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace PharmacyInventoryDispensingSystem.Application.Features.Patients.Dtos
{
    public sealed record PatientResponseDto(
        Guid Id,
        string DocumentId,
        PatientDocumentType DocumentType,
        string FullName,
        DateTime DateOfBirth,
        int Age,
        string PhoneNumber,
        DateTimeOffset CreatedAtUtc);


}
