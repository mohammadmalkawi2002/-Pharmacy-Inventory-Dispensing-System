using PharmacyInventoryDispensingSystem.Domain.Common;
using System.ComponentModel.DataAnnotations;

namespace PharmacyInventoryDispensingSystem.Domain.Entities;


public class MedicineBatch : SoftDeletableEntity
{

    public Guid MedicineId { get; set; }

    public string BatchNumber   { get; set; } = string.Empty;

    public DateTime ExpiryDate { get; set; }

    public int QuantityInStock { get; set; }
    public DateTimeOffset ReceivedAt { get; set; } 

    public Medicine Medicine { get; set; } = null!;

    public ICollection<StockMovement> StockMovements { get; set; } = [];

    public ICollection<DispenseItem> DispenseItems { get; set; } = [];

    
    
    public bool IsExpired(DateTime asOf) => ExpiryDate.Date < asOf.Date;

        
    /// Checks if the requested quantity can be allocated from this batch as of a specific date.
    public bool CanAllocate(int requestedQuantity, DateTime asOf) =>
        requestedQuantity > 0 && QuantityInStock >= requestedQuantity && !IsExpired(asOf);
}
