using MediatR;
using PharmacyInventoryDispensingSystem.Application.Common.Interfaces;
using PharmacyInventoryDispensingSystem.Domain.Common.Results;
using System;
using System.Collections.Generic;
using System.Text;

namespace PharmacyInventoryDispensingSystem.Application.Features.SecurityManager.Authentication.Commands.ChangePassword
{
    public sealed class ChangePasswordCommandHandler(IIdentityService identityService)
     : IRequestHandler<ChangePasswordCommand, Result<Success>>
    {
        public Task<Result<Success>> Handle(ChangePasswordCommand request, CancellationToken cancellationToken)
        {
            return identityService.ChangePasswordAsync(
                request.CurrentPassword,
                request.NewPassword,
                cancellationToken);
        }
    }
}
