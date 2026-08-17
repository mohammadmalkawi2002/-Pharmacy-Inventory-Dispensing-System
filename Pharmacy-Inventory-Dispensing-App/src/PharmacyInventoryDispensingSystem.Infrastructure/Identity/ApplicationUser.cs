using Microsoft.AspNetCore.Identity;
using PharmacyInventoryDispensingSystem.Domain.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace PharmacyInventoryDispensingSystem.Infrastructure.Identity
{
    public class ApplicationUser:IdentityUser
{
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public bool IsActive { get; set; } = true;

    }
}
