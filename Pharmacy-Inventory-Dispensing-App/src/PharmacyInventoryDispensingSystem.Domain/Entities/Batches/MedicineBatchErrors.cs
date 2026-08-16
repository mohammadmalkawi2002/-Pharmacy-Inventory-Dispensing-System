using System;
using System.Collections.Generic;
using System.Text;
using PharmacyInventoryDispensingSystem.Domain.Common;
using PharmacyInventoryDispensingSystem.Domain.Common.Results;
namespace PharmacyInventoryDispensingSystem.Domain.Entities.Batches
{
    public static class MedicineBatchErrors
    {
        public static Error MedicineBatchIdRequired => Error.Validation(
              code: "Batch.Id.Required",
              description: "Batch Id is  Required");

        public static Error MedicineIdRequired => Error.Validation(
              code: "Batch.MedicineId.Required",
              description: "Medicine Id is  Required");



        public static Error BatchNumberRequired =>
    Error.Validation(
        code: "Batch.BatchNumber.Required",
        description: "MedicineBatch  BatchNumber is required.");




        public static Error InitialQuantityInvalid => Error.Validation
            (code: "Batch.InitialQuantityInvalid", description: "Received quantity must be greater than zero.");


        public static Error InValidExpiryDate => Error.Validation
            (code: "Batch.ExpiryDate.Invalid", description: "ExpiryDate should be future on receive.");

        public static Error InsufficientStock(Guid? batchId, int requested, int available) =>
            Error.Conflict(
                "Batch.InsufficientStock",
                batchId is { } id
                    ? $"Insufficient stock in batch '{id}': requested {requested}, available {available}. (§13.4, §21)"
                    : $"Insufficient stock: requested {requested}, available {available}. (§13.4, §21)");
        public static Error AdjustmentQuantityZero =>
           Error.Validation("Batch.AdjustmentQuantityZero", "Adjustment quantity cannot be zero.");

        public static Error AdjustmentReasonRequired =>
            Error.Validation("Batch.AdjustmentReasonRequired", "A reason is required for stock adjustments. ");




    }
}
