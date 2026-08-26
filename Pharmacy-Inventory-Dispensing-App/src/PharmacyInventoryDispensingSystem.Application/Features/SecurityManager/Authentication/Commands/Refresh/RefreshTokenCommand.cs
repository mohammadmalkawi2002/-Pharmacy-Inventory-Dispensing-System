using MediatR;
using PharmacyInventoryDispensingSystem.Application.Features.SecurityManager.Authentication.DTOs;
using PharmacyInventoryDispensingSystem.Domain.Common.Results;
using System;
using System.Collections.Generic;
using System.Text;

namespace PharmacyInventoryDispensingSystem.Application.Features.SecurityManager.Authentication.Commands.Refresh
{
   public sealed record RefreshTokenCommand(string RefreshToken): IRequest<Result<AuthenticationResponse>>;
}
