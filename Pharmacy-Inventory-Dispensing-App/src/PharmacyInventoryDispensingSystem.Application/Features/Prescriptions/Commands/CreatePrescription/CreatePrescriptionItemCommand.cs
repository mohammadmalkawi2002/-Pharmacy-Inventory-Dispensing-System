using System;
using System.Collections.Generic;
using System.Text;

namespace PharmacyInventoryDispensingSystem.Application.Features.Prescriptions.Commands.CreatePrescription
{
    public sealed record CreatePrescriptionItemCommand(
        Guid MedicineId,
        int QuantityPrescribed,
        int MaxFillCount,
        string? DosageInstructions);
}
