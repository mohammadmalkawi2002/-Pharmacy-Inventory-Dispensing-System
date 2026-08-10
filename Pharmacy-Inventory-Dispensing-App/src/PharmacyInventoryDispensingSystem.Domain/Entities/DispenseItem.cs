using PharmacyInventoryDispensingSystem.Domain.Common;

namespace PharmacyInventoryDispensingSystem.Domain.Entities;

public class DispenseItem:BaseAuditableEntity
{

  

    public Guid DispenseId { get; set; }

    public Guid PrescriptionItemId { get; set; }

    public Guid MedicineBatchId { get; set; }

    public int Quantity { get; set; }

    public Dispense Dispense { get; set; } = null!;

    public PrescriptionItem PrescriptionItem { get; set; } = null!;

    public MedicineBatch MedicineBatch { get; set; } = null!;
}
