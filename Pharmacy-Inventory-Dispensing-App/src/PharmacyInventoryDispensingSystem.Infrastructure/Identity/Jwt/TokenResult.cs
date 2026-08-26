using System;
using System.Collections.Generic;
using System.Text;

namespace PharmacyInventoryDispensingSystem.Infrastructure.Identity.Jwt
{
    public sealed record TokenResult(
    string AccessToken,
    DateTimeOffset AccessTokenExpiresAtUtc);
}
