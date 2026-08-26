using MediatR;
using PharmacyInventoryDispensingSystem.Application.Common.Interfaces;
using PharmacyInventoryDispensingSystem.Domain.Common.Results;
using System;
using System.Collections.Generic;
using System.Text;

namespace PharmacyInventoryDispensingSystem.Application.Features.SecurityManager.Authentication.Commands.ForgotPassword
{
    public sealed class ForgotPasswordCommandHandler(IIdentityService identityService)
      : IRequestHandler<ForgotPasswordCommand, Result<Success>>
    {
        public Task<Result<Success>> Handle(ForgotPasswordCommand request, CancellationToken cancellationToken)
        {
            return identityService.ForgotPasswordAsync(
                request.Email,
                cancellationToken);
        }
    }
}