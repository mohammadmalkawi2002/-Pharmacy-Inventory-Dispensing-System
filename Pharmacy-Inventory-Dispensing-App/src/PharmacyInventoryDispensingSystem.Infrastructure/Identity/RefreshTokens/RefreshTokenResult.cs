using PharmacyInventoryDispensingSystem.Domain.Entities.Identity;
using System;
using System.Collections.Generic;
using System.Text;

namespace PharmacyInventoryDispensingSystem.Infrastructure.Identity.RefreshTokens
{
    public sealed record RefreshTokenResult(
    RefreshToken Entity,
    string PlainTextToken);
}
