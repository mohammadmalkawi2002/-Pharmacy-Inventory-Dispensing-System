using MediatR;
using PharmacyInventoryDispensingSystem.Application.Features.SecurityManager.Authentication.DTOs;
using PharmacyInventoryDispensingSystem.Domain.Common.Results;
using System;
using System.Collections.Generic;
using System.Text;

namespace PharmacyInventoryDispensingSystem.Application.Features.SecurityManager.Authentication.Commands.Register
{
    public sealed record RegisterUserCommand(
    string Email,
    string Password,
    string FirstName,
    string LastName,
    string Role)
    : IRequest<Result<AuthenticationResponse>>;
}
