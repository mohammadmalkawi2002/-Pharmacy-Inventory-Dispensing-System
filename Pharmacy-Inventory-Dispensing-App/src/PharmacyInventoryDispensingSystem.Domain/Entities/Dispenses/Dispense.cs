using PharmacyInventoryDispensingSystem.Domain.Common;
using PharmacyInventoryDispensingSystem.Domain.Entities.Prescriptions;

namespace PharmacyInventoryDispensingSystem.Domain.Entities.Dispenses;

public class Dispense : AuditableEntity
{
    public Guid PrescriptionId { get; set; }

    public string PharmacistId { get; set; } = string.Empty;

    public DateTimeOffset DispensedAt { get; set; }

    public string? Notes { get; set; }

    public Prescription Prescription { get; set; } = null!;

    public ICollection<DispenseItem> Items { get; set; } = [];

    public Dispense()
    {
    }
}
