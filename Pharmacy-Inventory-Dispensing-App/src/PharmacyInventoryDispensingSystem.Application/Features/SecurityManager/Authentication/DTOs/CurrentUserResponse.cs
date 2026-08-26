using System;
using System.Collections.Generic;
using System.Text;

namespace PharmacyInventoryDispensingSystem.Application.Features.SecurityManager.Authentication.DTOs
{
    /// <summary>
    /// GET /api/v1/auth/me
    /// </summary>
    /// <param name="UserId"></param>
    /// <param name="Email"></param>
    /// <param name="FirstName"></param>
    /// <param name="LastName"></param>
    /// <param name="Roles"></param>
    /// <param name="Permissions"></param>
    public sealed record CurrentUserResponse
        (
            string UserId,
            string Email,
             string FirstName,
            string LastName,
            IReadOnlyCollection<string> Roles,
            IReadOnlyCollection<string> Permissions
        );
}
