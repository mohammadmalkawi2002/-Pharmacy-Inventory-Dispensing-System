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
    /// Strength of the medicine, e.g. 500mg, 250mg.
    /// </summary>
    public string Strength { get; set; } = string.Empty;


    public MedicineForm Form { get; set; }

    /// <summary>
    /// Unit used for inventory, e.g. Box, Vial.
    /// </summary>
    public string Unit { get; set; } = string.Empty;

    /// <summary>
    /// Minimum stock level that triggers a low-stock warning.
    /// </summary>
    public int ReorderLevel { get; set; }

    /// <summary>
    /// represents the current available quantity of the medicine.
    /// ex] Panadol => QuantityInStock = 25,     No batch-level breakdown exists.
    /// </summary>
    public int QuantityInStock {  get;private set; }


    public bool IsActive { get; set; }

    public ICollection<PrescriptionItem> PrescriptionItems { get; set; }
        = new List<PrescriptionItem>();
   

  
        public Result<Success> EnsureStockAvailable(int quantity)
        {
        if (quantity <= 0)
            return MedicineErrors.InvalidQuantity;

        if (quantity > QuantityInStock)
            return MedicineErrors.InsufficientStock;

        return Result.Success;
        }

}
