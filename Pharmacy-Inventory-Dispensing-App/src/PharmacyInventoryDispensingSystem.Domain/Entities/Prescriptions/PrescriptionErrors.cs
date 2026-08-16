using PharmacyInventoryDispensingSystem.Domain.Common.Results;
using System;
using System.Collections.Generic;
using System.Text;

namespace PharmacyInventoryDispensingSystem.Domain.Entities.Prescriptions
{
    public static class PrescriptionErrors
    {
        public static Error PrescriptionIdRequired => Error.Validation(
            code: "Prescription.Id.Required",
            description: "Prescription Id is  Required");


        public static Error NumberRequired =>
    Error.Validation(
        code: "Prescription.PrescriptionNumber.Required",
        description: "Prescription Number is required.");

        public static Error PatientNameRequired =>
         Error.Validation("Prescription.PatientNameRequired", "Patient name is required.");

        public static Error DoctorIdRequired =>
            Error.Validation("Prescription.DoctorIdRequired", "A prescribing doctor is required.");
        public static Error NoItems =>
    Error.Validation("Prescription.NoItems", "A prescription must have at least one item.");

        public static Error CannotArchiveDispensed =>
        Error.Conflict("Prescription.CannotArchiveDispensed", 
            "Cannot archive a prescription that has been dispensed.");

        public static Error InvalidValidityPeriod => Error.Validation(
            "Prescription.InvalidValidityPeriod", "ValidFrom must be on or before ValidTo. ");

        public static Error MaxRefillsNegative =>
           Error.Validation("Prescription.MaxRefillsNegative", "Max refills cannot be negative.");


    }

}

