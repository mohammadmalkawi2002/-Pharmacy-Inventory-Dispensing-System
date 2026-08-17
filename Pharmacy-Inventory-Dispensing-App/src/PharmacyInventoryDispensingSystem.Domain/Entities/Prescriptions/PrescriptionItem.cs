using PharmacyInventoryDispensingSystem.Domain.Common;
using PharmacyInventoryDispensingSystem.Domain.Entities.Dispenses;
using PharmacyInventoryDispensingSystem.Domain.Entities.Medicines;

namespace PharmacyInventoryDispensingSystem.Domain.Entities.Prescriptions;

public class PrescriptionItem : AuditableEntity
{
    public Guid PrescriptionId { get; set; }

    public Guid MedicineId { get; set; }

    public int QuantityPrescribed { get; set; }

    public int QuantityDispensed { get; set; }

    public string? DosageInstructions { get; set; }

    public Prescription Prescription { get; set; } = null!;

    public Medicine Medicine { get; set; } = null!;

    public ICollection<DispenseItem> DispenseItems { get; set; } = [];

    public int RemainingQuantity => QuantityPrescribed - QuantityDispensed;

    public bool CanDispense(int requestedQuantity) =>
        requestedQuantity > 0 && requestedQuantity <= RemainingQuantity;

    public PrescriptionItem()
    {
    }
}
