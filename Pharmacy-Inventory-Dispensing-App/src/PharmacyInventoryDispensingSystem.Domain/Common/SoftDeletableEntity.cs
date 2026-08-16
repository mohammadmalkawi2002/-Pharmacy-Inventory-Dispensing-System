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
    public DateTimeOffset?RestoreAtUtc { get; set; }

    public void Delete(DateTime deletedAt)
    {
        DeletedAtUtc = deletedAt;
        IsDeleted = true;
    }

    public void Restore()
    {
        RestoreAtUtc = DateTime.UtcNow;
        IsDeleted = false;
    }
}
