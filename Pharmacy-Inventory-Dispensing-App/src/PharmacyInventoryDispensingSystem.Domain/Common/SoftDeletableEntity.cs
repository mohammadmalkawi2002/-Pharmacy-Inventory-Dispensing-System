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
    public void Delete()
    {
      
        IsDeleted = true;
    }
    public void Restore()
    {
        IsDeleted = false;
    }
}
