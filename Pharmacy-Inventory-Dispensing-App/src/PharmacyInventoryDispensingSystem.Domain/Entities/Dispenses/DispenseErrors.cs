using PharmacyInventoryDispensingSystem.Domain.Common.Results;
using System;
using System.Collections.Generic;
using System.Text;

namespace PharmacyInventoryDispensingSystem.Domain.Entities.Dispenses
{
    public static class DispenseErrors
    {
        public static readonly Error DispenseNotFound = Error.NotFound(
    "Dispense.NotFound",
    "The dispensing record was not found.");
        public static readonly Error PrescriptionNotFound = Error.NotFound(
            "Dispense.PrescriptionNotFound",
            "The prescription was not found or does not match the patient document ID.");

        public static readonly Error PatientUnavailable = Error.Conflict(
            "Dispense.PatientUnavailable",
            "The patient is unavailable.");

        public static readonly Error PrescriptionCancelled = Error.Conflict(
            "Dispense.PrescriptionCancelled",
            "A cancelled prescription cannot be dispensed.");

        public static readonly Error PrescriptionExpired = Error.Conflict(
            "Dispense.PrescriptionExpired",
            "An expired prescription cannot be dispensed.");

        public static readonly Error PrescriptionNotYetValid = Error.Conflict(
            "Dispense.PrescriptionNotYetValid",
            "The prescription is not valid yet.");

        public static readonly Error PrescriptionItemNotFound = Error.NotFound(
            "Dispense.PrescriptionItemNotFound",
            "One or more selected items do not belong to this prescription.");

        public static Error MedicineUnavailable(string medicineName) =>
            Error.Conflict(
                "Dispense.MedicineUnavailable",
                $"Medicine '{medicineName}' is archived or inactive.");

        public static Error NoFillsRemaining(string medicineName) =>
            Error.Conflict(
                "Dispense.NoFillsRemaining",
                $"Medicine '{medicineName}' has no remaining fills.");
    }
}