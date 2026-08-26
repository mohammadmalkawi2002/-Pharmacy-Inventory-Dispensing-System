using Microsoft.AspNetCore.Authorization;
using System;
using System.Collections.Generic;
using System.Text;

namespace PharmacyInventoryDispensingSystem.Infrastructure.Identity.Authorization
{
    /// <summary>
    /// Its represent => I have an authorization Condition,
    /// the user authenticated must the owner of this Prescription
    /// </summary>
    public sealed class PrescriptionOwnerRequirement:IAuthorizationRequirement
    {
    }
}
