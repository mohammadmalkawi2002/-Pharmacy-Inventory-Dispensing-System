namespace PharmacyInventoryDispensingSystem.Domain.Common;

public abstract class AuditableEntity:Entity
{


    public DateTimeOffset CreatedAtUtc { get; set; }

    public string? CreatedBy { get; set; }

    public DateTimeOffset? UpdatedAtUtc { get; set; }

    public string? UpdatedBy { get; set; }

    protected AuditableEntity()
    {

    }

    
}

 
   

   

