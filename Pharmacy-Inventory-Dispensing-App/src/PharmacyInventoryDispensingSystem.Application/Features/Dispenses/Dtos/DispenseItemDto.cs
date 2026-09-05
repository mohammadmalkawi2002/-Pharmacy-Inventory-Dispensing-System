using PharmacyInventoryDispensingSystem.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace PharmacyInventoryDispensingSystem.Application.Features.Dispenses.Dtos
{
    /// <summary>
    /// Represent each Item or medicine has dispensed 
    /// </summary>
    /// <param name="Id"></param>
    /// <param name="PrescriptionItemId"></param>
    /// <param name="MedicineId"></param>
    /// <param name="MedicineCode"></param>
    /// <param name="MedicineName"></param>
    /// <param name="Strength"></param>
    /// <param name="StockUnit"></param>
    /// <param name="Quantity"></param>
    /// <param name="DosageInstructions"></param>
    public sealed record DispenseItemDto(
     Guid Id,
     Guid PrescriptionItemId,
     Guid MedicineId,
     string MedicineCode,
     string MedicineName,
     string Strength,
     StockUnit StockUnit,
     int Quantity,
     string? DosageInstructions);
}
