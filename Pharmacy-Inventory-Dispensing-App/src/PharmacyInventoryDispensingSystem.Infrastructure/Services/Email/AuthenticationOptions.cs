using System;
using System.Collections.Generic;
using System.Text;

namespace PharmacyInventoryDispensingSystem.Infrastructure.Services.Email
{
    public sealed class AuthenticationOptions
    {
        public const string SectionName = "Authentication";

        public string FrontendUrl { get; set; } = string.Empty;
    }
}
