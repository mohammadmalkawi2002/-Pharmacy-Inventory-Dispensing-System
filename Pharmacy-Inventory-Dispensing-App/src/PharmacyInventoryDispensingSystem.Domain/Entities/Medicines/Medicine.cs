using PharmacyInventoryDispensingSystem.Domain.Common;
using PharmacyInventoryDispensingSystem.Domain.Common.Results;
using PharmacyInventoryDispensingSystem.Domain.Entities.Prescriptions;
using PharmacyInventoryDispensingSystem.Domain.Enums;
using System.ComponentModel.DataAnnotations;

namespace PharmacyInventoryDispensingSystem.Domain.Entities.Medicines;

public sealed class Medicine : SoftDeletableEntity
{
    /// <summary>
    /// Unique identifier used to search for the medicine.
    /// </summary>
    public string Code { get; set; } = string.Empty;

    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Strength of the medicine, e.g. 500 mg, 250 mg.
    /// This describes medicine strength, not inventory quantity.
    /// </summary>
    public string Strength { get; set; } = string.Empty;

    public MedicineForm Form { get; set; }

    /// <summary>
    /// Base unit used to track stock and dispense the medicine.
    /// Example: Tablet, Capsule, Bottle, Vial.
    /// </summary>
    public StockUnit StockUnit { get; set; }

    /// <summary>
    /// Package unit used when receiving stock.
    /// Example: Box, Pack.
    /// </summary>
    public PackageUnit PackageUnit { get; set; }

    /// <summary>
    /// Number of stock units contained in one package.
    /// Example: 1 Box = 20 Tablets.
    /// </summary>
    public int UnitsPerPackage { get; set; }

    /// <summary>
    /// Minimum stock level, expressed in StockUnit.
    /// </summary>
    public int ReorderLevel { get; set; }

    /// <summary>
    /// Current available quantity, expressed in StockUnit.
    /// Example: StockUnit = Tablet and QuantityInStock = 200
    /// means 200 tablets are currently available.
    /// </summary>
    public int QuantityInStock { get; private set; }

    public bool IsActive { get; set; }

    public ICollection<PrescriptionItem> PrescriptionItems { get; set; }
        = new List<PrescriptionItem>();


    public Result<Success> IncreaseStock(int quantity) 
    {
        if (quantity <= 0)
            return MedicineErrors.InvalidQuantity;

        QuantityInStock += quantity;

        return Result.Success;
    }


    public Result<Success> DecreaseStock(int quantity)
    {
        if (quantity <= 0)
            return MedicineErrors.InvalidQuantity;

        if (quantity > QuantityInStock)
            return MedicineErrors.InsufficientStock;

        QuantityInStock -= quantity;

        return Result.Success;
    }

    public Result<Success> EnsureStockAvailable(int quantity)
    {
        if (quantity <= 0)
            return MedicineErrors.InvalidQuantity;

        if (quantity > QuantityInStock)
            return MedicineErrors.InsufficientStock;

        return Result.Success;
    }

}
