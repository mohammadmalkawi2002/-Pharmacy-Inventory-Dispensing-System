using PharmacyInventoryDispensingSystem.Domain.Common;
using PharmacyInventoryDispensingSystem.Domain.Entities.Prescriptions;

namespace PharmacyInventoryDispensingSystem.Domain.Entities.Dispenses;

public class DispenseItem : AuditableEntity
{



    public Guid DispenseId { get; set; }

    public Guid PrescriptionItemId { get; set; }


    public int Quantity { get; set; }

    public Dispense Dispense { get; set; } = null!;

    public PrescriptionItem PrescriptionItem { get; set; } = null!;

}
