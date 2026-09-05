using MediatR;
using Microsoft.Extensions.Logging;
using PharmacyInventoryDispensingSystem.Application.Common.Interfaces;
using PharmacyInventoryDispensingSystem.Domain.Common.Results;
using System;
using System.Collections.Generic;
using System.Text;

namespace PharmacyInventoryDispensingSystem.Application.Features.Users.Commands.ActivateUser
{
    public sealed class ActivateUserCommandHandler(
        IStaffUserService staffUserService,
        ILogger<ActivateUserCommandHandler> logger)
        : IRequestHandler<ActivateUserCommand, Result<Updated>>
    {
        public async Task<Result<Updated>> Handle(
            ActivateUserCommand request,
            CancellationToken cancellationToken)
        {
            logger.LogInformation(
                "Admin activating staff user. UserId: {UserId}",
                request.UserId);

            var result = await staffUserService.ActivateAsync(request.UserId, cancellationToken);

            if (result.IsSuccess)
            {
                logger.LogInformation(
                    "Staff user activated successfully. UserId: {UserId}",
                    request.UserId);
            }
            else
            {
                logger.LogWarning(
                    "Staff user activation failed. UserId: {UserId}, Error: {ErrorCode}",
                    request.UserId,
                    result.TopError.Code);
            }

            return result;
        }
    }
}
