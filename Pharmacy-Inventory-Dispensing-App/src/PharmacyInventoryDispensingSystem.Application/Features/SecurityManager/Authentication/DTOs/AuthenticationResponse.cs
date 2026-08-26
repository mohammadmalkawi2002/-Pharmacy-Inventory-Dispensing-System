using System;
using System.Collections.Generic;
using System.Text;

namespace PharmacyInventoryDispensingSystem.Application.Features.SecurityManager.Authentication.DTOs
{
    public sealed record AuthenticationResponse(
     string UserId,
     string Email,
     string FirstName,
     string LastName,
     IReadOnlyCollection<string> Roles,
     IReadOnlyCollection<string> Permissions,
     string AccessToken,
     DateTimeOffset AccessTokenExpiresAtUtc,
     string RefreshToken,
     DateTimeOffset RefreshTokenExpiresAtUtc
        );
}
