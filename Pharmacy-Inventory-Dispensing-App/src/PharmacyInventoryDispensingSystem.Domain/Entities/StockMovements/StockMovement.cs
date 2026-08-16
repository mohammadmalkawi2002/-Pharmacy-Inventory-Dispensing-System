using PharmacyInventoryDispensingSystem.Domain.Common;
using PharmacyInventoryDispensingSystem.Domain.Common.Results;
using PharmacyInventoryDispensingSystem.Domain.Entities.Batches;
using PharmacyInventoryDispensingSystem.Domain.Enums;
using System.ComponentModel.DataAnnotations;

namespace PharmacyInventoryDispensingSystem.Domain.Entities.StockMovements;

public class StockMovement : AuditableEntity
{
    public Guid MedicineBatchId { get; private set; }

    public MovementType MovementType { get; private set; }

    /// <summary>
    /// Positive for stock in, negative for stock out.
    /// </summary>
    public int QuantityChange { get; private set; }

    public string? Reason { get; set; }

    public MedicineBatch MedicineBatch { get; set; } = null!;



    private StockMovement()
    {
        //Ef
    }



    private StockMovement(Guid medicineBatchId, MovementType movementType, int quantityChange, string? reason)
        
    {
        MedicineBatchId = medicineBatchId;
        MovementType = movementType;
        QuantityChange = quantityChange;
        Reason = reason;
    }

    /// <summary>
    /// Internal: because its only visible inside  Doamin Layer =>
    /// stockMovement is created as a result of a business operation on MedicineBatch 
    /// (such as Receive, Dispense, Adjustment, or Expired).
    /// </summary>
    internal static Result<StockMovement> Create(Guid medicineBatchId, MovementType movementType, int quantityChange, string? reason)
    {

        if (medicineBatchId == Guid.Empty)
            return StockMovementErrors.BatchIdRequired;

        if (quantityChange == 0)
            return StockMovementErrors.QuantityChangeZero;

        if (movementType == MovementType.Adjustment && string.IsNullOrWhiteSpace(reason))
            return StockMovementErrors.AdjustmentReasonRequired;

        var normalizedReason = string.IsNullOrWhiteSpace(reason) ? null : reason.Trim();
        return new StockMovement(medicineBatchId, movementType, quantityChange, normalizedReason);
    }
}






    

