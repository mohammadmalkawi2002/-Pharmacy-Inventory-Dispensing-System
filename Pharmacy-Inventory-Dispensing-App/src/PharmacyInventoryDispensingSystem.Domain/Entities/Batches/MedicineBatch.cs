using PharmacyInventoryDispensingSystem.Domain.Common;
using PharmacyInventoryDispensingSystem.Domain.Common.Results;
using PharmacyInventoryDispensingSystem.Domain.Entities.Dispenses;
using PharmacyInventoryDispensingSystem.Domain.Entities.Medicines;
using PharmacyInventoryDispensingSystem.Domain.Entities.StockMovements;
using PharmacyInventoryDispensingSystem.Domain.Enums;

namespace PharmacyInventoryDispensingSystem.Domain.Entities.Batches;


public class MedicineBatch : SoftDeletableEntity
{

  
    public Guid MedicineId { get; private set; }
    public Medicine Medicine { get; private set; } = null!;

    public string BatchNumber { get; private set; } = string.Empty;

    public DateTime ExpiryDate { get; private set; }

    public int QuantityInStock { get; private set; }//100

    public DateTimeOffset ReceivedAt { get; private set; }


    private readonly List<StockMovement> _stockMovements = [];
    public IReadOnlyCollection<StockMovement> StockMovements => _stockMovements.AsReadOnly();

    private readonly List<DispenseItem> _dispenseItems = [];
    public IReadOnlyCollection<DispenseItem> DispenseItems => _dispenseItems.AsReadOnly();



    private MedicineBatch()
    {
        // EF Core materialization only.
    }

    private MedicineBatch(Guid id,Guid medicineId, string batchNumber, DateTime expiryDate, int Quantity, DateTimeOffset receivedAt )
        :base(id)
    {
        MedicineId = medicineId;
        BatchNumber = batchNumber;
        ExpiryDate = expiryDate.Date;
        QuantityInStock = Quantity;
        ReceivedAt = receivedAt ;
    }

    ///Methods for this entity: Create(), Adjust() ,CanAllocate()
    


    /// <summary>
    /// Creates a new medicine batch with its initial stock
    /// and records the corresponding Receive stock movement.
    /// </summary>
    /// <returns> Batch result if success  or erros</returns>
    public static Result<MedicineBatch> Create(
        Guid id, Guid medicineId, string batchNumber, DateTime expiryDate, 
        int Quantity, DateTimeOffset receivedAt) 
    {
        if (id == Guid.Empty)
            return MedicineBatchErrors.MedicineBatchIdRequired;

        if (medicineId == Guid.Empty)
            return MedicineBatchErrors.MedicineIdRequired;

        if (string.IsNullOrWhiteSpace(batchNumber))
            return MedicineBatchErrors.BatchNumberRequired;

        if (expiryDate.Date <= DateTime.UtcNow.Date)
            return MedicineBatchErrors.InValidExpiryDate;

        if(Quantity <=0 )
            return MedicineBatchErrors.InitialQuantityInvalid;

        var batch=new MedicineBatch(id, medicineId, batchNumber.Trim(), expiryDate, Quantity,receivedAt);

        //create or audit movement:

        var movementResult = StockMovement.Create(batch.Id, MovementType.Receive, +Quantity, reason: null);

        if (movementResult.IsError)
            return movementResult.Errors;

        batch._stockMovements.Add(movementResult.Value);

        //TODO: Later Maybe Add event:

        return batch;
    }

    /// <summary>
    /// Add to an exisiting batch medicines
    /// </summary>
    /// <returns></returns>
    public static Result<Updated> Receive()
    {
        return Error.Unexpected(
            "Batch.Receive.NotImplemented",
            "Receiving additional stock into an existing batch is not implemented.");
    }


    /// <summary>
    /// is responsible for removing a specific quantity from a MedicineBatch during the dispensing process.
    /// </summary>
    /// <returns></returns>
    //public Result<StockMovement> Allocate(int quantity, DateTime asOf) 
    //{
    //    if (!CanAllocate(quantity, asOf))
    //        return MedicineBatchErrors.InsufficientStock;


    //    var movementResult=StockMovement.Create(Id,MovementType.Dispense, -quantity, reason: null);
    //    if(movementResult.IsError)
    //        return movementResult.Errors;

    //    //after we auidt the movemnet we should decrese the StockQuentiy:
    //    QuantityInStock-=quantity;

    //    _stockMovements.Add(movementResult.Value);

    //    if(QuantityInStock==0 )


    //}


    /// <summary>
    /// Manual correction (damage, recount, etc. 17.2). 
    /// A reason is mandatory and the
    /// resulting quantity can never go negative (§15 rule 3).
    /// </summary>
    /// 
    public  Result<StockMovement> Adjust(int quantityChange,string reason) 
    {
        if (quantityChange == 0)
            return MedicineBatchErrors.AdjustmentQuantityZero;

        if (string.IsNullOrWhiteSpace(reason))
            return MedicineBatchErrors.AdjustmentReasonRequired;

        var newQuantity = QuantityInStock + quantityChange;
        if (newQuantity < 0)
            return MedicineBatchErrors.InsufficientStock(Id, Math.Abs(quantityChange), QuantityInStock);

        var movementResult = StockMovement.Create(Id, MovementType.Adjustment, quantityChange, reason);
        if (movementResult.IsError)
            return movementResult.Errors;

        QuantityInStock = newQuantity;
        _stockMovements.Add(movementResult.Value);
        return movementResult.Value;
    }

    public bool IsExpired(DateTime asOf) => ExpiryDate.Date < asOf.Date;

        
    /// Checks if the requested quantity can be allocated from this batch as of a specific date.
    public bool CanAllocate(int requestedQuantity, DateTime asOf) =>
        requestedQuantity > 0 && QuantityInStock >= requestedQuantity && !IsExpired(asOf);

}
