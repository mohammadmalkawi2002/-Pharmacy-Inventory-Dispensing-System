using System;
using System.Collections.Generic;
using System.Text;

namespace PharmacyInventoryDispensingSystem.Infrastructure.Identity.Settings
{

    public sealed class JwtSettings
    {
        public string Issuer { get; init; } = string.Empty;

        public string Audience { get; init; } = string.Empty;

        public string SecretKey { get; init; } = string.Empty;

        public int AccessTokenExpirationMinutes { get; init; } = 15;
    }
}
