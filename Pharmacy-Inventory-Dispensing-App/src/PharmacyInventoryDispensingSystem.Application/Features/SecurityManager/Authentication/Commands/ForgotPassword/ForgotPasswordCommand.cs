using MediatR;
using PharmacyInventoryDispensingSystem.Domain.Common.Results;
using System;
using System.Collections.Generic;
using System.Text;

namespace PharmacyInventoryDispensingSystem.Application.Features.SecurityManager.Authentication.Commands.ForgotPassword
{
    public sealed record ForgotPasswordCommand(string Email) : IRequest<Result<Success>>;
}
