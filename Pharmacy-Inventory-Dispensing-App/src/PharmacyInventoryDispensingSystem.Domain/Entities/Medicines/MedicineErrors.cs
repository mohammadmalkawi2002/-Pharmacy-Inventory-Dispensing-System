using PharmacyInventoryDispensingSystem.Domain.Common.Results;
using System;
using System.Collections.Generic;
using System.Text;

namespace PharmacyInventoryDispensingSystem.Domain.Entities.Medicines
{
    public static class MedicineErrors
    {
         public static Error MedicineIdRequired => Error.Validation(
            code: "MedicineIdErrors.MedicineIdRequired",
            description: "Medicine Id is  Required");

        public static Error InvalidQuantity =>
        Error.Validation(
        "Medicine.InvalidQuantity",
        "Quantity must be greater than zero.");

        public static Error InsufficientStock =>
            Error.Conflict(
                "Medicine.InsufficientStock",
                "The requested quantity is greater than the available stock.");
        public static Error CodeRequired =>
    Error.Validation(
        code: "Medicine.Code.Required",
        description: "Medicine code is required.");

        

        public static Error NameRequired =>
            Error.Validation(
                code: "Medicine.Name.Required",
                description: "Medicine name is required.");

       

        public static Error StrengthRequired =>
            Error.Validation(
                code: "Medicine.Strength.Required",
                description: "Medicine strength is required.");

       

        public static Error ReorderLevelNegative =>
            Error.Validation(
                code: "Medicine.ReorderLevel.Negative",
                description: "Reorder level cannot be negative.");

        public static Error InvalidForm =>
            Error.Validation(
                code: "Medicine.Form.Invalid",
                description: "The provided medicine form is invalid.");
        public static Error AlreadyActive =>
            Error.Conflict(code: "Medicine.AlreadyActive", description: "Medicine is Already Active ");

        public static Error AlreadyInactive => 
            Error.Conflict(code: "Medicine.AlreadyInactive", description: "Medicine  is already inactive.");

        
        public static Error Archived(string code) => Error.Conflict(
            code: "Medicine.Archived", description: $"Medicine with code : `{code}`  is archived and cannot be prescribed");


        public static Error Inactive(string code) =>
            Error.Conflict("Medicine.Inactive", $"Medicine '{code}' is inactive and cannot be prescribed.");
    }
}
