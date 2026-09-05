using System;
using System.Collections.Generic;
using System.Text;

namespace PharmacyInventoryDispensingSystem.Application.Features.Users.Dtos
{
    /// <summary>
    /// Response DTO for a single staff user. Role is a single string because
    /// the pharmacy system assigns exactly one role per staff account.
    /// </summary>
    public sealed record StaffUserDto(
        string Id,
        string FirstName,
        string LastName,
        string Email,
        string Role,
        bool IsActive,
        DateTimeOffset CreatedAtUtc);
}
