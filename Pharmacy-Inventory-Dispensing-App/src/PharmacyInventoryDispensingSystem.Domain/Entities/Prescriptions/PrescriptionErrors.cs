using PharmacyInventoryDispensingSystem.Domain.Common.Results;
using System;
using System.Collections.Generic;
using System.Text;

namespace PharmacyInventoryDispensingSystem.Domain.Entities.Prescriptions
{
    public static class PrescriptionErrors
    {

        public static Error NotFound(Guid prescriptionId) =>
          Error.NotFound(
              code: "Prescription.NotFound",
              description: $"Prescription with Id '{prescriptionId}' was not found.");


        public static Error LookupNotFound =>
      Error.NotFound(
          code: "Prescription.LookupNotFound",
          description: "No prescription was found for the provided prescription number and patient document ID.");

        public static readonly Error Forbidden =
        Error.Forbidden(
            code: "Prescription.Forbidden",
            description: "You are not permitted to access this prescription.");


        public static Error CannotUpdateDispensed =>
    Error.Conflict(
        code: "Prescription.CannotUpdateDispensed",
        description: "A prescription with dispensing history cannot be updated.");

        public static Error DuplicateMedicine(Guid medicineId) =>
            Error.Conflict(
                code: "Prescription.DuplicateMedicine",
                description: $"Medicine with Id '{medicineId}' cannot appear more than once in the same prescription.");

        public static Error CannotUpdateInActive =>
            Error.Conflict(
                code: "Prescription.CannotUpdateInActive",
                description: "An InActive prescription cannot be updated.");

        

        public static Error AlreadyCancelled =>
            Error.Conflict(
                code: "Prescription.AlreadyCancelled",
                description: "The prescription is already cancelled.");

        public static Error CannotCancelExpired =>
            Error.Conflict(
                code: "Prescription.CannotCancelExpired",
                description: "An expired prescription cannot be cancelled.");
    }


}



