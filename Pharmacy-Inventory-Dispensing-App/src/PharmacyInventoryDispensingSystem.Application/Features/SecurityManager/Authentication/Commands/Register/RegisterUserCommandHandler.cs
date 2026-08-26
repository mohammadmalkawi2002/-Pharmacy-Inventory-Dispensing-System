using MediatR;
using PharmacyInventoryDispensingSystem.Application.Common.Interfaces;
using PharmacyInventoryDispensingSystem.Application.Features.SecurityManager.Authentication.DTOs;
using PharmacyInventoryDispensingSystem.Domain.Common.Results;
using System;
using System.Collections.Generic;
using System.Text;

namespace PharmacyInventoryDispensingSystem.Application.Features.SecurityManager.Authentication.Commands.Register
{
    public sealed class RegisterUserCommandHandler(IIdentityService identityService) : 
        IRequestHandler<RegisterUserCommand, Result<AuthenticationResponse>>
    {
        public Task<Result<AuthenticationResponse>> Handle(RegisterUserCommand request, CancellationToken cancellationToken)
        {
            return identityService.RegisterAsync(
                request.Email,
                request.Password,
                request.FirstName,
                request.LastName,
                request.Role,
                cancellationToken
                );
        }
    }
}
