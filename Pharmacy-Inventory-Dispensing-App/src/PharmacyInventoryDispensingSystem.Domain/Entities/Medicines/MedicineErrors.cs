using PharmacyInventoryDispensingSystem.Domain.Common.Results;
using System;
using System.Collections.Generic;
using System.Text;

namespace PharmacyInventoryDispensingSystem.Domain.Entities.Medicines
{
    public static class MedicineErrors
    {
        
        public static Error NotFound(Guid medicineId) =>
            Error.NotFound(
                code: "Medicine.NotFound",
                description: $"Medicine with ID '{medicineId}' was not found.");

        public static Error StockConfigurationCannotBeChanged =>
    Error.Conflict(
        "Medicine.StockConfigurationCannotBeChanged",
        "Stock configuration cannot be changed because the medicine has stock or is referenced by a prescription.");

        public static Error NotFoundByCode(string code) =>
            Error.NotFound(
                code: "Medicine.NotFoundByCode",
                description: $"Medicine with code '{code}' was not found.");

        public static Error CodeConflict =>
            Error.Conflict(
                code: "Medicine.Code.Conflict",
                description: "A medicine with the same code already exists.");

        public static Error InvalidQuantity =>
            Error.Validation(
                "Medicine.InvalidQuantity",
                "Quantity must be greater than zero.");

        public static Error InsufficientStock =>
            Error.Conflict(
                "Medicine.InsufficientStock",
                "The requested quantity is greater than the available stock.");


        public static Error AlreadyActive =>
            Error.Conflict(code: "Medicine.AlreadyActive", description: "Medicine is already active.");

        public static Error AlreadyInactive => 
            Error.Conflict(code: "Medicine.AlreadyInactive", description: "Medicine is already inactive.");

        public static Error AlreadyArchived(Guid medicineId) =>
            Error.Conflict(
                code: "Medicine.AlreadyArchived",
                description: $"Medicine with ID '{medicineId}' is already archived.");

        public static Error NotArchived(Guid medicineId) =>
            Error.Conflict(
                code: "Medicine.NotArchived",
                description: $"Medicine with ID '{medicineId}' is not archived.");


        public static Error Inactive(string code) =>
            Error.Conflict("Medicine.Inactive", $"Medicine '{code}' is inactive .");
    }
}
