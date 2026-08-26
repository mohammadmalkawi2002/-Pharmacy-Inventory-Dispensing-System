using MediatR;
using PharmacyInventoryDispensingSystem.Application.Common.Interfaces;
using PharmacyInventoryDispensingSystem.Application.Features.SecurityManager.Authentication.DTOs;
using PharmacyInventoryDispensingSystem.Domain.Common.Results;
using System;
using System.Collections.Generic;
using System.Text;

namespace PharmacyInventoryDispensingSystem.Application.Features.SecurityManager.Authentication.Commands.Login
{
    public sealed class LoginCommandHandler(IIdentityService identityService) :
        IRequestHandler<LoginCommand, Result<AuthenticationResponse>>
    {
        public Task<Result<AuthenticationResponse>> Handle(LoginCommand request, CancellationToken cancellationToken)
        {
            return identityService.LoginAsync(
                request.Email, 
                request.Password,
                cancellationToken
                );

        }
    }
}
