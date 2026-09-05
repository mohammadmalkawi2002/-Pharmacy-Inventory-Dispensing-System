using MediatR;
using Microsoft.Extensions.Logging;
using PharmacyInventoryDispensingSystem.Application.Common.Interfaces;
using PharmacyInventoryDispensingSystem.Domain.Common.Results;
using System;
using System.Collections.Generic;
using System.Text;

namespace PharmacyInventoryDispensingSystem.Application.Features.Users.Commands.DeactivateUser
{
    public sealed class DeactivateUserCommandHandler(
        IStaffUserService staffUserService,
        ILogger<DeactivateUserCommandHandler> logger)
        : IRequestHandler<DeactivateUserCommand, Result<Updated>>
    {
        public async Task<Result<Updated>> Handle(
            DeactivateUserCommand request,
            CancellationToken cancellationToken)
        {
            logger.LogInformation(
                "Admin deactivating staff user. UserId: {UserId}",
                request.UserId);

            var result = await staffUserService.DeactivateAsync(request.UserId, cancellationToken);

            if (result.IsSuccess)
            {
                logger.LogInformation(
                    "Staff user deactivated successfully. UserId: {UserId}",
                    request.UserId);
            }
            else
            {
                logger.LogWarning(
                    "Staff user deactivation failed. UserId: {UserId}, Error: {ErrorCode}",
                    request.UserId,
                    result.TopError.Code);
            }

            return result;
        }
    }
}
