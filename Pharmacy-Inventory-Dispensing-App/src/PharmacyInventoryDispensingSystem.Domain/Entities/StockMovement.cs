using PharmacyInventoryDispensingSystem.Domain.Common;
using PharmacyInventoryDispensingSystem.Domain.Enums;
using System.ComponentModel.DataAnnotations;

namespace PharmacyInventoryDispensingSystem.Domain.Entities;

public class StockMovement : BaseAuditableEntity
{
    public Guid MedicineBatchId { get; set; }

    public MovementType MovementType { get; set; }

    /// <summary>
    /// Positive for stock in, negative for stock out.
    /// </summary>
    public int QuantityChange { get; set; }

    public string? Reason { get; set; }

    public MedicineBatch MedicineBatch { get; set; } = null!;



    //TODO:  I want to use Result Pattern for the Create method, but I am not sure how to implement it.

    //public static StockMovement Create(
    //    Guid medicineBatchId,
    //    MovementType movementType,
    //    int quantityChange,
    //    Guid createdBy,
    //    string? reason = null)
    //{
    //    if (quantityChange == 0)
    //    {
    //        throw new ArgumentException("Quantity change cannot be zero.", nameof(quantityChange));
    //    }

    //    if (movementType == MovementType.Adjustment && string.IsNullOrWhiteSpace(reason))
    //    {
    //        throw new ArgumentException("Reason is required for adjustment movements.", nameof(reason));
    //    }

    //    return new StockMovement
    //    {
    //        MedicineBatchId = medicineBatchId,
    //        MovementType = movementType,
    //        QuantityChange = quantityChange,
    //        Reason = reason?.Trim(),
    //        CreatedBy = createdBy,
    //        CreatedAt = DateTime.UtcNow
    //    };


    //}
}
