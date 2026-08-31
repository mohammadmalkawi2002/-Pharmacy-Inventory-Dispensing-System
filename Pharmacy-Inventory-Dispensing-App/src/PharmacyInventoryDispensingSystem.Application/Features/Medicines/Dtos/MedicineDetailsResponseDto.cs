using PharmacyInventoryDispensingSystem.Application.Features.Medicines.Common;
using PharmacyInventoryDispensingSystem.Domain.Enums;
using System;

namespace PharmacyInventoryDispensingSystem.Application.Features.Medicines.Dtos
{
    public sealed record MedicineDetailsResponseDto(
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
     bool IsActive,
     DateTimeOffset CreatedAtUtc,
     string? CreatedBy,
     DateTimeOffset? UpdatedAtUtc,
     string? UpdatedBy);
}
