using MediatR;
using PharmacyInventoryDispensingSystem.Domain.Common.Results;
using System;
using System.Collections.Generic;
using System.Text;

namespace PharmacyInventoryDispensingSystem.Application.Features.SecurityManager.Authentication.Commands.ChangePassword
{
    public sealed record ChangePasswordCommand(string CurrentPassword, string NewPassword)
        : IRequest<Result<Success>>;

}
