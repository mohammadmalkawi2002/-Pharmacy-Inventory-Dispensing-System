using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using PharmacyInventoryDispensingSystem.Application.Common.Interfaces.Authorization;
using PharmacyInventoryDispensingSystem.Domain.Entities.Prescriptions;
using System;
using System.Collections.Generic;
using System.Text;

namespace PharmacyInventoryDispensingSystem.Infrastructure.Identity.Authorization
{
    public sealed class PrescriptionAuthorizationService (
        IAuthorizationService authorizationService,
        IHttpContextAccessor httpContextAccessor)
        : IPrescriptionAuthorizationService
    {
        public async Task<bool> CanAccessAsync(Prescription prescription, CancellationToken cancellationToken = default)
        {
            var user = httpContextAccessor.HttpContext?.User;

            if(user?.Identity?.IsAuthenticated != true)
            {
                return false;
            }

            var result=await authorizationService.AuthorizeAsync(
                user,
                prescription,
                new PrescriptionOwnerRequirement());


            return result.Succeeded;
        }
    }
}
