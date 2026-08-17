namespace PharmacyInventoryDispensingSystem.Domain.Common;

public abstract class SoftDeletableEntity: AuditableEntity
{

    protected SoftDeletableEntity()
    {
        
    }


    protected SoftDeletableEntity(Guid id)
        :base(id)
    {
        
    }
    public bool IsDeleted { get; set; }

    public DateTimeOffset? DeletedAtUtc { get; set; }

    public string? DeletedBy { get; set; }

    public DateTimeOffset? RestoreAtUtc { get; set; }

    public void Delete(string? deletedBy, DateTimeOffset? deletedAt = null)
    {
        IsDeleted = true;
        DeletedAtUtc = deletedAt ?? DateTimeOffset.UtcNow;
        DeletedBy = deletedBy;
        RestoreAtUtc = null;
    }

    public void Restore()
    {
        IsDeleted = false;
        RestoreAtUtc = DateTimeOffset.UtcNow;
        DeletedAtUtc = null;
        DeletedBy = null;
    }
}
