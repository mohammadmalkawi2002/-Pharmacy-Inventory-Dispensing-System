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
    }
}
