using MediatR;
using Microsoft.Extensions.Logging;
using PharmacyInventoryDispensingSystem.Application.Common.Interfaces;
using PharmacyInventoryDispensingSystem.Domain.Common.Results;
using System;
using System.Collections.Generic;
using System.Text;

namespace PharmacyInventoryDispensingSystem.Application.Features.Users.Commands.UpdateUser
{
    public sealed class UpdateUserCommandHandler(
        IStaffUserService staffUserService,
        ILogger<UpdateUserCommandHandler> logger)
        : IRequestHandler<UpdateUserCommand, Result<Updated>>
    {
        public async Task<Result<Updated>> Handle(
            UpdateUserCommand request,
            CancellationToken cancellationToken)
        {
            logger.LogInformation(
                "Admin updating staff user. UserId: {UserId}",
                request.UserId);

            var result = await staffUserService.UpdateAsync(
                request.UserId,
                request.FirstName,
                request.LastName,
                request.Email,
                request.Role,
                cancellationToken);

            if (result.IsSuccess)
            {
                logger.LogInformation(
                    "Staff user updated successfully. UserId: {UserId}",
                    request.UserId);
            }
            else
            {
                logger.LogWarning(
                    "Staff user update failed. UserId: {UserId}, Error: {ErrorCode}",
                    request.UserId,
                    result.TopError.Code);
            }

            return result;
        }
    }
}
