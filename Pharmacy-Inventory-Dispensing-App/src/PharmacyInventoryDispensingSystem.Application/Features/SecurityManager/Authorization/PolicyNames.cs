using System;
using System.Collections.Generic;
using System.Text;

namespace PharmacyInventoryDispensingSystem.Application.Features.SecurityManager.Authorization
{
    public static class PolicyNames
    {
        // Role-based policies
        public const string AdminOnly = "AdminOnly";
        public const string ReceptionistOrAdmin = "ReceptionistOrAdmin";
        public const string DoctorOrAdmin = "DoctorOrAdmin";
        public const string PharmacistOrAdmin = "PharmacistOrAdmin";

        // Permission-based policies
        public const string CanReadUsers = "CanReadUsers";
        public const string CanCreateUsers = "CanCreateUsers";
        public const string CanUpdateUsers = "CanUpdateUsers";
        public const string CanActivateUsers = "CanActivateUsers";
        public const string CanDeactivateUsers = "CanDeactivateUsers";

        public const string CanReadPatients = "CanReadPatients";
        public const string CanCreatePatients = "CanCreatePatients";
        public const string CanUpdatePatients = "CanUpdatePatients";

        public const string CanReadMedicines = "CanReadMedicines";
        public const string CanCreateMedicines = "CanCreateMedicines";
        public const string CanUpdateMedicines = "CanUpdateMedicines";
        public const string CanActivateMedicines = "CanActivateMedicines";
        public const string CanDeactivateMedicines = "CanDeactivateMedicines";
        public const string CanReadLowStockMedicines = "CanReadLowStockMedicines";

        public const string CanReadPrescriptions = "CanReadPrescriptions";
        public const string CanCreatePrescriptions = "CanCreatePrescriptions";
        public const string CanUpdatePrescriptions = "CanUpdatePrescriptions";
        public const string CanCancelPrescriptions = "CanCancelPrescriptions";
        public const string CanLookupPrescriptions = "CanLookupPrescriptions";

        public const string CanReadDispenses = "CanReadDispenses";
        public const string CanCreateDispenses = "CanCreateDispenses";
    }
}
