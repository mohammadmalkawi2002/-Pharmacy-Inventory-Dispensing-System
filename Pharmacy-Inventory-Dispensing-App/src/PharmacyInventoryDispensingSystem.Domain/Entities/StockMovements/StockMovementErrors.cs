using PharmacyInventoryDispensingSystem.Domain.Common.Results;
using System;
using System.Collections.Generic;
using System.Text;

namespace PharmacyInventoryDispensingSystem.Domain.Entities.StockMovements
{
    public static class StockMovementErrors
    {

        public static Error IdRequired => Error.Validation
            ("StockMovement.Id.Required", "StockMovement Id is required ");

        public static Error BatchIdRequired => Error.Validation
            ("StockMovement.BatchId.Required", "A stock movement must reference a batch. ");

        public static Error QuantityChangeZero => Error.Validation
            ("StockMovement.QuantityChangeZero", "Quantity change cannot be zero.");

        public static Error AdjustmentReasonRequired =>
          Error.Validation("StockMovement.AdjustmentReasonRequired", "Reason is required for adjustment movements.");



    }
}
