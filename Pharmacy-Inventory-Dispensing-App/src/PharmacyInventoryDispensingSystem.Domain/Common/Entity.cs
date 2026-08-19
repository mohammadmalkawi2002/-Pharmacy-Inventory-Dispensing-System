using System.ComponentModel.DataAnnotations.Schema;

namespace PharmacyInventoryDispensingSystem.Domain.Common;

public abstract class Entity
{
    public Guid Id { get; set; } = Guid.CreateVersion7();



    protected Entity()
    { }

   

}
