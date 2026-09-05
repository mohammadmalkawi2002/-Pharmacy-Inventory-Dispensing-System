using PharmacyInventoryDispensingSystem.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace PharmacyInventoryDispensingSystem.Application.Features.Medicines.Dtos
{
    public sealed record MedicineLookupDto(
    Guid Id,
    string Code,
    string Name,
    string Strength,
    MedicineForm Form,
    StockUnit StockUnit);
}
