namespace PharmacyInventoryDispensingSystem.Domain.Common;

public abstract class AuditableEntity:Entity
{

    protected AuditableEntity()
    {
        
    }
    protected AuditableEntity(Guid id)
        :base(id)
    {

    }

 
   
    public DateTimeOffset CreatedAtUtc { get; set; }

    public string? CreatedBy { get; set; }

    public DateTimeOffset? UpdatedAtUtc { get; set; }

    public string? UpdatedBy { get; set; }
}
   

