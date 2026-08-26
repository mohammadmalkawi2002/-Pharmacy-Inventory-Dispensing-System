using MediatR;
using PharmacyInventoryDispensingSystem.Application.Common.Interfaces;
using PharmacyInventoryDispensingSystem.Domain.Common.Results;
using System;
using System.Collections.Generic;
using System.Text;

namespace PharmacyInventoryDispensingSystem.Application.Features.SecurityManager.Authentication.Commands.ResetPassword
{
    public sealed class ResetPasswordCommandHandler(IIdentityService identityService)
    : IRequestHandler<ResetPasswordCommand, Result<Success>>
    {
        public Task<Result<Success>> Handle(ResetPasswordCommand request, CancellationToken cancellationToken)
        {
            return identityService.ResetPasswordAsync(
                request.Email,
                request.Token,
                request.NewPassword,
                cancellationToken);
        }
    }
}
