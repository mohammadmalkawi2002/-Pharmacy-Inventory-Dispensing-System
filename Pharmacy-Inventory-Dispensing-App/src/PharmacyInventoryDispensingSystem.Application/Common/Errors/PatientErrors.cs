using PharmacyInventoryDispensingSystem.Domain.Common.Results;
using System;
using System.Collections.Generic;
using System.Text;

namespace PharmacyInventoryDispensingSystem.Application.Common.Errors
{
    public static class PatientErrors
    {
        public static Error NotFound(Guid patientId) =>
            Error.NotFound(
           code: "Patient.NotFound",
           description: $"Patient with ID '{patientId}' was not found.");

        public static Error NotFoundByDocumentId=>
             Error.NotFound(
        code: "Patient.NotFoundByDocumentId",
        description: "Patient with the provided document ID was not found.");
        public static Error DocumentIdConflict =>
            Error.Conflict(
         code: "Patient.DocumentId.Conflict",
         description: "A patient with the same document ID already exists.");


        public static Error AlreadyArchived(Guid patientId) =>
      Error.Conflict(
          code: "Patient.AlreadyArchived",
          description: $"Patient with ID '{patientId}' is already archived.");

        public static Error NotArchived(Guid patientId) =>
            Error.Conflict(
                code: "Patient.NotArchived",
                description: $"Patient with ID '{patientId}' is not archived . patient already Active");
    }
}
