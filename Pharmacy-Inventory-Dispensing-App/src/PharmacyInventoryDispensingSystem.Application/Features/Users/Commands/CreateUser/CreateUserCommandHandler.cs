using MediatR;
using Microsoft.Extensions.Logging;
using PharmacyInventoryDispensingSystem.Application.Common.Interfaces;
using PharmacyInventoryDispensingSystem.Application.Features.Users.Dtos;
using PharmacyInventoryDispensingSystem.Domain.Common.Results;
using System;
using System.Collections.Generic;
using System.Text;

namespace PharmacyInventoryDispensingSystem.Application.Features.Users.Commands.CreateUser
{
    public sealed class CreateUserCommandHandler(
        IStaffUserService staffUserService,
        ILogger<CreateUserCommandHandler> logger)
        : IRequestHandler<CreateUserCommand, Result<StaffUserDto>>
    {
        public async Task<Result<StaffUserDto>> Handle(
            CreateUserCommand request,
            CancellationToken cancellationToken)
        {
            logger.LogInformation(
                "Admin creating staff user. Email: {Email}, Role: {Role}",
                request.Email,
                request.Role);

            var result = await staffUserService.CreateAsync(
                request.FirstName,
                request.LastName,
                request.Email,
                request.Password,
                request.Role,
                cancellationToken);

            if (result.IsSuccess)
            {
                logger.LogInformation(
                    "Staff user created successfully. UserId: {UserId}, Role: {Role}",
                    result.Value.Id,
                    result.Value.Role);
            }
            else
            {
                logger.LogWarning(
                    "Staff user creation failed. Email: {Email}, Error: {ErrorCode}",
                    request.Email,
                    result.TopError.Code);
            }

            return result;
        }
    }
}
