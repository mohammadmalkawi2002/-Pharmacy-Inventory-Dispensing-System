using MediatR;
using PharmacyInventoryDispensingSystem.Domain.Common.Results;
using System;
using System.Collections.Generic;
using System.Text;

namespace PharmacyInventoryDispensingSystem.Application.Features.SecurityManager.Authentication.Commands.Logout
{
    public sealed record LogoutCommand(string RefreshToken):IRequest<Result<Success>>;
    
    
}
