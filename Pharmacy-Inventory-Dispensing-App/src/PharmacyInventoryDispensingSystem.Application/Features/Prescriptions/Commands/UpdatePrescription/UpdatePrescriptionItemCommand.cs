using System;
using System.Collections.Generic;
using System.Text;

namespace PharmacyInventoryDispensingSystem.Application.Features.Prescriptions.Commands.UpdatePrescription
{
    public sealed record UpdatePrescriptionItemCommand(
        Guid MedicineId,
        int QuantityPrescribed,
        int MaxFillCount,
        string? DosageInstructions);
}
