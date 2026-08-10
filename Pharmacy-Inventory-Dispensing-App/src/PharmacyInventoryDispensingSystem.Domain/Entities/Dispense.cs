using PharmacyInventoryDispensingSystem.Domain.Common;
using System.ComponentModel.DataAnnotations;

namespace PharmacyInventoryDispensingSystem.Domain.Entities;

public class Dispense : BaseAuditableEntity
{
   

    public Guid PrescriptionId { get; set; }

    public string PharmacistId { get; set; }

    public DateTimeOffset DispensedAt { get; set; }

    public string? Notes { get; set; }

    public Prescription Prescription { get; set; } = null!;

    public ICollection<DispenseItem> Items { get; set; } = [];
}
