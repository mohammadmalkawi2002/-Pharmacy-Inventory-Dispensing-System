using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using PharmacyInventoryDispensingSystem.Application.Common.Interfaces;

namespace PharmacyInventoryDispensingSystem.Infrastructure.Identity;

internal sealed class CurrentUserService(IHttpContextAccessor httpContextAccessor) : ICurrentUser
{
    public string? UserId => httpContextAccessor.HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier);

    public bool IsAuthenticated =>
        httpContextAccessor.HttpContext?.User.Identity?.IsAuthenticated ?? false;
}
