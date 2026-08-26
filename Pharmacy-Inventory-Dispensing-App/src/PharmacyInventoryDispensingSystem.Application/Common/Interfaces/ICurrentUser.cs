using System;
using System.Collections.Generic;
using System.Text;

namespace PharmacyInventoryDispensingSystem.Application.Common.Interfaces
{
    /// <summary>
    /// Lets Application handlers access the authenticated user without depending on HttpContext
    /// </summary>
    public interface ICurrentUser
    {
        string? Id { get; }
        string? Email { get; }
        bool IsAuthenticated { get; }
        bool IsInRole(string role);
        bool HasPermission(string permission);
        IReadOnlyCollection<string> Roles { get; }
        IReadOnlyCollection<string> Permissions { get; }

    }
}
