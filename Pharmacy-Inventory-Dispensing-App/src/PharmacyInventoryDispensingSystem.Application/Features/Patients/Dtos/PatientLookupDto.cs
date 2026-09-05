using System;
using System.Collections.Generic;
using System.Text;

namespace PharmacyInventoryDispensingSystem.Application.Features.Patients.Dtos
{

    public sealed record PatientLookupDto(
        Guid Id,
        string DocumentId,
        string FullName);
}
