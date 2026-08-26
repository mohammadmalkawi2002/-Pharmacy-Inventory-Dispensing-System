using System;
using System.Collections.Generic;
using System.Text;

namespace PharmacyInventoryDispensingSystem.Application.Features.SecurityManager.Authorization
{
    /// <summary>
    /// Defines the custom claim type used for permission claims in JWTs and Identity role claims.
    /// </summary>
    public static class ApplicationClaimTypes
    {
        public const string Permission = "permission";
    }
}
