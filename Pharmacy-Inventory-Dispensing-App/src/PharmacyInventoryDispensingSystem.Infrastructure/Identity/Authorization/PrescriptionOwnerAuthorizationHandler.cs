using Microsoft.AspNetCore.Authorization;
using PharmacyInventoryDispensingSystem.Application.Common.Interfaces;
using PharmacyInventoryDispensingSystem.Application.Features.SecurityManager.Authorization;
using PharmacyInventoryDispensingSystem.Domain.Entities.Prescriptions;
using System;
using System.Collections.Generic;
using System.Text;

namespace PharmacyInventoryDispensingSystem.Infrastructure.Identity.Authorization
{
    public sealed class PrescriptionOwnerAuthorizationHandler(ICurrentUser currentUser)
        : AuthorizationHandler<PrescriptionOwnerRequirement, Prescription>
    {
        protected override Task HandleRequirementAsync(
            AuthorizationHandlerContext context,
            PrescriptionOwnerRequirement requirement,
            Prescription resource)
        {
            //Admin can access any prescription:

            if (currentUser.IsInRole(RoleNames.Admin))
            {
                context.Succeed(requirement);

                return Task.CompletedTask;
            }


            // Doctor can access only their own prescriptions.

            if (currentUser.IsInRole(RoleNames.Doctor) 
                && currentUser.Id== resource.Id.ToString())
            {
                context.Succeed(requirement);
            }
            return Task.CompletedTask;
        }
    }
}
