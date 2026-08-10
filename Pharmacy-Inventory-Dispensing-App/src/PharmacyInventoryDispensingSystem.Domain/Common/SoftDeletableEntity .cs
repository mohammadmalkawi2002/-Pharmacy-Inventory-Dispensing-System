namespace PharmacyInventoryDispensingSystem.Domain.Common;

public abstract class SoftDeletableEntity: BaseAuditableEntity
{
    public bool IsDeleted { get; set; }

    public DateTimeOffset? DeletedAt { get; set; }
}
