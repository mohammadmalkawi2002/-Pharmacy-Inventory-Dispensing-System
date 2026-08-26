using MediatR;
using PharmacyInventoryDispensingSystem.Domain.Common.Results;
using System;
using System.Collections.Generic;
using System.Text;

namespace PharmacyInventoryDispensingSystem.Application.Features.SecurityManager.Authentication.Commands.ResetPassword
{
    public sealed record ResetPasswordCommand(
        string Email,
        string Token,
        string NewPassword) : IRequest<Result<Success>>;
}
