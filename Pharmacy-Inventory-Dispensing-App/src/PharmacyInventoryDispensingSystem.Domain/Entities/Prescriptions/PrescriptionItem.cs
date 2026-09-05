using PharmacyInventoryDispensingSystem.Domain.Common;
using PharmacyInventoryDispensingSystem.Domain.Entities.Dispenses;
using PharmacyInventoryDispensingSystem.Domain.Entities.Medicines;
using System.ComponentModel.DataAnnotations;

namespace PharmacyInventoryDispensingSystem.Domain.Entities.Prescriptions;

public class PrescriptionItem : AuditableEntity
{

    public Guid PrescriptionId { get; set; }

    public Prescription Prescription { get; set; } = null!;

    public Guid MedicineId { get; set; }

    public Medicine Medicine { get; set; } = null!;

    /// <summary>
    /// Quantity to dispense on each fill.(per fill)
    /// The quantity is expressed in the medicine's StockUnit.
    /// </summary>
    public int QuantityPrescribed { get; set; }

    /// <summary>
    /// Maximum number of allowed fills for this(Medicine) or prescription item.
    /// </summary>
    public int MaxFillCount { get; set; }

    /// <summary>
    /// Number of fills already used for this prescription item.
    /// </summary>
    public int FillUsedCount { get; set; }

    public string? DosageInstructions { get; set; }

    public ICollection<DispenseItem> DispenseItems { get; set; } = [];

    public bool HasFillsRemaining =>
        FillUsedCount < MaxFillCount;

    public int RemainingFillCount =>
    MaxFillCount - FillUsedCount;
}
