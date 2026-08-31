using PharmacyInventoryDispensingSystem.Application.Features.Medicines.Common;
using PharmacyInventoryDispensingSystem.Application.Features.Medicines.Dtos;
using PharmacyInventoryDispensingSystem.Domain.Entities.Medicines;
using System;
using System.Collections.Generic;
using System.Linq;

namespace PharmacyInventoryDispensingSystem.Application.Features.Medicines.Mappers
{
    public static class MedicineMapper
    {
        public static MedicineResponseDto ToDto(this Medicine entity)
        {
            ArgumentNullException.ThrowIfNull(entity);

            return new MedicineResponseDto(
                Id: entity.Id,
                Code: entity.Code,
                Name: entity.Name,
                Strength: entity.Strength,
                Form: entity.Form,
                StockUnit: entity.StockUnit,
                PackageUnit: entity.PackageUnit,
                UnitsPerPackage: entity.UnitsPerPackage,
                QuantityInStock: entity.QuantityInStock,
                ReorderLevel: entity.ReorderLevel,
                StockStatus: CalculateStockStatus(entity.QuantityInStock,
                                                  entity.ReorderLevel),
                IsActive: entity.IsActive);

        }

        public static MedicineDetailsResponseDto ToDetailsDto(this Medicine entity)
        {
            ArgumentNullException.ThrowIfNull(entity);

            return new MedicineDetailsResponseDto(
                Id: entity.Id,
                Code: entity.Code,
                Name: entity.Name,
                Strength: entity.Strength,
                Form: entity.Form,
                StockUnit: entity.StockUnit,
                PackageUnit: entity.PackageUnit,
                UnitsPerPackage: entity.UnitsPerPackage,
                QuantityInStock: entity.QuantityInStock,
                ReorderLevel: entity.ReorderLevel,
                StockStatus: CalculateStockStatus(
                    entity.QuantityInStock,
                    entity.ReorderLevel),
                IsActive: entity.IsActive,
                CreatedAtUtc: entity.CreatedAtUtc,
                CreatedBy: entity.CreatedBy,
                UpdatedAtUtc: entity.UpdatedAtUtc,
                UpdatedBy: entity.UpdatedBy);
        }

        public static List<MedicineResponseDto> ToDtos(this IEnumerable<Medicine> entities)
        {
            ArgumentNullException.ThrowIfNull(entities);

            return [
                .. entities.Select(entity => entity.ToDto())
            ];
        }

        private static StockStatus CalculateStockStatus(int quantityInStock, int reorderLevel)
        {
            if (quantityInStock == 0)
            {
                return StockStatus.OutOfStock;
            }

            if (quantityInStock <= reorderLevel)
            {
                return StockStatus.LowStock;
            }

            return StockStatus.Normal;
        }
    }
}
