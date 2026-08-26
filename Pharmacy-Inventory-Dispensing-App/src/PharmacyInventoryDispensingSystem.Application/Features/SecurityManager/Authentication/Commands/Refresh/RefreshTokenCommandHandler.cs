using MediatR;
using PharmacyInventoryDispensingSystem.Application.Common.Interfaces;
using PharmacyInventoryDispensingSystem.Application.Features.SecurityManager.Authentication.DTOs;
using PharmacyInventoryDispensingSystem.Domain.Common.Results;
using System;
using System.Collections.Generic;
using System.Text;

namespace PharmacyInventoryDispensingSystem.Application.Features.SecurityManager.Authentication.Commands.Refresh
{
    public sealed class RefreshTokenCommandHandler(IIdentityService identityService)
        : IRequestHandler<RefreshTokenCommand, Result<AuthenticationResponse>>
    {
        public Task<Result<AuthenticationResponse>> Handle(RefreshTokenCommand request, CancellationToken cancellationToken)
        {
            return identityService.RefreshAsync(request.RefreshToken, cancellationToken);
        }
    }
}
