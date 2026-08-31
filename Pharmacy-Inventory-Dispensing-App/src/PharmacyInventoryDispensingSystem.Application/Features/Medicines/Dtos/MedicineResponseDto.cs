using PharmacyInventoryDispensingSystem.Application.Features.Medicines.Common;
using PharmacyInventoryDispensingSystem.Domain.Enums;
using System;

namespace PharmacyInventoryDispensingSystem.Application.Features.Medicines.Dtos
{
    public sealed record MedicineResponseDto(
    Guid Id,
    string Code,
    string Name,
    string Strength,
    MedicineForm Form,
    StockUnit StockUnit,
    PackageUnit PackageUnit,
    int UnitsPerPackage,
    int QuantityInStock,
    int ReorderLevel,
    StockStatus StockStatus,
    bool IsActive);
}
