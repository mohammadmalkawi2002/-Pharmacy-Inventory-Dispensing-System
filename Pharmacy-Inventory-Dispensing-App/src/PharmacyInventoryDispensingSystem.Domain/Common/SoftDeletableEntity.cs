namespace PharmacyInventoryDispensingSystem.Domain.Common;

public abstract class SoftDeletableEntity: AuditableEntity
{

    protected SoftDeletableEntity()
    {
        
    }
    public bool IsDeleted { get; set; }

    public DateTimeOffset? DeletedAtUtc { get; set; }
    public DateTimeOffset? RestoredAtUtc { get; set; }
    public string? DeletedBy { get; set; }


    public void Delete(DateTime deletedAt)
    {
        DeletedAtUtc = deletedAt;
        IsDeleted = true;
    }

    public void Restore()
    {
        RestoredAtUtc = DateTime.UtcNow;
        IsDeleted = false;
    }
}
