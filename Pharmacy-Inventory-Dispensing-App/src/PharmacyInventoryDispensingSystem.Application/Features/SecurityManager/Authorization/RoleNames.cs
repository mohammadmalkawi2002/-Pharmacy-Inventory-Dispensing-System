using System;
using System.Collections.Generic;
using System.Text;

namespace PharmacyInventoryDispensingSystem.Application.Features.SecurityManager.Authorization
{
    public static class RoleNames
    {
        public const string Admin = "Admin";
        public const string Receptionist = "Receptionist";
        public const string Doctor = "Doctor";
        public const string Pharmacist = "Pharmacist";

        public static readonly string[] All =
        [
            Admin,
        Receptionist,
        Doctor,
        Pharmacist
        ];
    }
}
