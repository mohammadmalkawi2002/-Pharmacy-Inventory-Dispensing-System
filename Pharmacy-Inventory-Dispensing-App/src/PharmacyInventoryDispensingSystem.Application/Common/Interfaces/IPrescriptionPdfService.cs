using PharmacyInventoryDispensingSystem.Application.Features.Prescriptions.Dtos;
using System;
using System.Collections.Generic;
using System.Text;

namespace PharmacyInventoryDispensingSystem.Application.Common.Interfaces
{
    public interface IPrescriptionPdfService
    {
        byte[] Generate(PrescriptionPdfDto prescriptionDto);
    }
}
