using System;
using System.Collections.Generic;
using System.Text;

namespace PharmacyInventoryDispensingSystem.Application.Features.Prescriptions.Dtos
{
    public sealed record PrescriptionPdfFileResult(
        byte[] FileBytes,
        string FileName,
        string ContentType);
}
