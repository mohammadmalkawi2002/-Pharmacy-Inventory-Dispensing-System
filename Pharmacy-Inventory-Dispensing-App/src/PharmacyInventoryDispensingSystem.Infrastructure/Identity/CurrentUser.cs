using Microsoft.AspNetCore.Http;
using PharmacyInventoryDispensingSystem.Application.Common.Interfaces;
using PharmacyInventoryDispensingSystem.Application.Features.SecurityManager.Authorization;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text;

namespace PharmacyInventoryDispensingSystem.Infrastructure.Identity
{
    public sealed class CurrentUser(IHttpContextAccessor httpContextAccessor) : ICurrentUser
    {
        private ClaimsPrincipal? User => httpContextAccessor.HttpContext?.User;
        public string? Id => User?.FindFirstValue(ClaimTypes.NameIdentifier);

        public string? Email => User?.FindFirstValue(ClaimTypes.Email);

        public bool IsAuthenticated => User?.Identity?.IsAuthenticated==true;

        public IReadOnlyCollection<string> Roles => User?.FindAll(ClaimTypes.Role)
                                                        .Select(claim=>claim.Value)
                                                         .ToArray()??[];

        public IReadOnlyCollection<string> Permissions => User?.FindAll(ApplicationClaimTypes.Permission)
                                                            .Select(claim=>claim.Value)
                                                                .ToArray()??[];

        public bool IsInRole(string role)
       => User?.IsInRole(role) == true;

        public bool HasPermission(string permission)
            => Permissions.Contains(permission);
    }
}
