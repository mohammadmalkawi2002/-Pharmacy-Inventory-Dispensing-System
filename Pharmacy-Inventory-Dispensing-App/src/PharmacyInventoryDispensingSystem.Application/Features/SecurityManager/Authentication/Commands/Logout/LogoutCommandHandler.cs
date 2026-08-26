using MediatR;
using PharmacyInventoryDispensingSystem.Application.Common.Interfaces;
using PharmacyInventoryDispensingSystem.Domain.Common.Results;
using System;
using System.Collections.Generic;
using System.Text;

namespace PharmacyInventoryDispensingSystem.Application.Features.SecurityManager.Authentication.Commands.Logout
{
    public sealed class LogoutCommandHandler(IIdentityService identityService)
    : IRequestHandler<LogoutCommand, Result<Success>>
    {
        public Task<Result<Success>> Handle(LogoutCommand request, CancellationToken cancellationToken)
        {
            return identityService.LogoutAsync(request.RefreshToken, cancellationToken);
        }
    }
}
