using Asp.Versioning;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PharmacyInventoryDispensingSystem.Application.Common.Models;
using PharmacyInventoryDispensingSystem.Application.Features.SecurityManager.Authorization;
using PharmacyInventoryDispensingSystem.Application.Features.Users.Commands.ActivateUser;
using PharmacyInventoryDispensingSystem.Application.Features.Users.Commands.CreateUser;
using PharmacyInventoryDispensingSystem.Application.Features.Users.Commands.DeactivateUser;
using PharmacyInventoryDispensingSystem.Application.Features.Users.Commands.UpdateUser;
using PharmacyInventoryDispensingSystem.Application.Features.Users.Dtos;
using PharmacyInventoryDispensingSystem.Application.Features.Users.Queries.GetUserById;
using PharmacyInventoryDispensingSystem.Application.Features.Users.Queries.GetUsers;
using PharmacyInventoryDispensingSystem.WebApi.Contracts.ApiResponse;
using PharmacyInventoryDispensingSystem.WebApi.Contracts.Requests.User;

namespace PharmacyInventoryDispensingSystem.WebApi.Controllers
{
    [ApiController]
    [Route("api/v{version:apiVersion}/users")]
    [ApiVersion("1.0")]
    [Tags("Users")]
    [Authorize]
    [Produces("application/json")]
    [ProducesResponseType<ApiErrorResponse>(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType<ApiErrorResponse>(StatusCodes.Status403Forbidden)]
    [ProducesResponseType<ApiErrorResponse>(StatusCodes.Status500InternalServerError)]
    public sealed class UsersController(ISender sender) : ApiController
    {
        // -----------------------------------------------------------------------
        // GET /api/v1/users
        // -----------------------------------------------------------------------

        [HttpGet]
        [Authorize(Policy = Permissions.Users.Read)]
        [ProducesResponseType<PaginatedList<StaffUserDto>>(StatusCodes.Status200OK)]
        [ProducesResponseType<ApiErrorResponse>(StatusCodes.Status400BadRequest)]
        [EndpointName("GetUsersV1")]
        [EndpointSummary("Gets paginated staff users")]
        [EndpointDescription(
            "Returns a paginated list of active and inactive staff users (Receptionist, Doctor, Pharmacist). " +
            "Admin accounts are excluded. Supports optional search and role filter.")]
        [MapToApiVersion("1.0")]
        public async Task<IActionResult> GetPaged(
            [FromQuery] GetUsersRequest request,
            CancellationToken cancellationToken)
        {
            var query = new GetUsersQuery(
                request.SearchTerm,
                request.Role,
                request.PageNumber,
                request.PageSize);

            var result = await sender.Send(query, cancellationToken);

            return result.Match(
                response => Ok(response),
                Problem);
        }

        // -----------------------------------------------------------------------
        // GET /api/v1/users/{userId}
        // -----------------------------------------------------------------------

        [HttpGet("{userId}", Name = "GetUserById")]
        [Authorize(Policy = Permissions.Users.Read)]
        [ProducesResponseType<StaffUserDto>(StatusCodes.Status200OK)]
        [ProducesResponseType<ApiErrorResponse>(StatusCodes.Status404NotFound)]
        [EndpointName("GetUserByIdV1")]
        [EndpointSummary("Gets a staff user by ID")]
        [EndpointDescription(
            "Returns detailed information about the specified staff user. " +
            "Returns 404 if the user does not exist or is an admin account.")]
        [MapToApiVersion("1.0")]
        public async Task<IActionResult> GetById(
            string userId,
            CancellationToken cancellationToken)
        {
            var query = new GetUserByIdQuery(userId);

            var result = await sender.Send(query, cancellationToken);

            return result.Match(
                response => Ok(response),
                Problem);
        }

        // -----------------------------------------------------------------------
        // POST /api/v1/users
        // -----------------------------------------------------------------------

        [HttpPost]
        [Authorize(Policy = Permissions.Users.Create)]
        [Consumes("application/json")]
        [ProducesResponseType<StaffUserDto>(StatusCodes.Status201Created)]
        [ProducesResponseType<ApiErrorResponse>(StatusCodes.Status400BadRequest)]
        [ProducesResponseType<ApiErrorResponse>(StatusCodes.Status409Conflict)]
        [EndpointName("CreateUserV1")]
        [EndpointSummary("Creates a new staff user")]
        [EndpointDescription(
            "Creates a new staff user (Receptionist, Doctor, or Pharmacist) with the provided initial password. " +
            "No authentication token is generated. Admin accounts cannot be created via this endpoint.")]
        [MapToApiVersion("1.0")]
        public async Task<IActionResult> CreateUser(
            [FromBody] CreateUserRequest request,
            CancellationToken cancellationToken)
        {
            var command = new CreateUserCommand(
                request.FirstName,
                request.LastName,
                request.Email,
                request.Password,
                request.Role);

            var result = await sender.Send(command, cancellationToken);

            return result.Match(
                response => CreatedAtAction(
                    nameof(GetById),
                    new { version = "1", userId = response.Id },
                    response),
                Problem);
        }

        // -----------------------------------------------------------------------
        // PUT /api/v1/users/{userId}
        // -----------------------------------------------------------------------

        [HttpPut("{userId}")]
        [Authorize(Policy = Permissions.Users.Update)]
        [Consumes("application/json")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType<ApiErrorResponse>(StatusCodes.Status400BadRequest)]
        [ProducesResponseType<ApiErrorResponse>(StatusCodes.Status404NotFound)]
        [ProducesResponseType<ApiErrorResponse>(StatusCodes.Status409Conflict)]
        [EndpointName("UpdateUserV1")]
        [EndpointSummary("Updates an existing staff user")]
        [EndpointDescription(
            "Updates the name, email, and role of an existing staff user. " +
            "Admin accounts cannot be modified via this endpoint.")]
        [MapToApiVersion("1.0")]
        public async Task<IActionResult> UpdateUser(
            string userId,
            [FromBody] UpdateUserRequest request,
            CancellationToken cancellationToken)
        {
            var command = new UpdateUserCommand(
                userId,
                request.FirstName,
                request.LastName,
                request.Email,
                request.Role);

            var result = await sender.Send(command, cancellationToken);

            return result.Match(
                _ => NoContent(),
                Problem);
        }

        // -----------------------------------------------------------------------
        // PATCH /api/v1/users/{userId}/activate
        // -----------------------------------------------------------------------

        [HttpPatch("{userId}/activate")]
        [Authorize(Policy = Permissions.Users.Activate)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType<ApiErrorResponse>(StatusCodes.Status404NotFound)]
        [ProducesResponseType<ApiErrorResponse>(StatusCodes.Status409Conflict)]
        [EndpointName("ActivateUserV1")]
        [EndpointSummary("Activates a staff user account")]
        [EndpointDescription(
            "Sets the staff user's IsActive flag to true. " +
            "Returns 409 if the account is already active. Admin accounts are protected.")]
        [MapToApiVersion("1.0")]
        public async Task<IActionResult> ActivateUser(
            string userId,
            CancellationToken cancellationToken)
        {
            var command = new ActivateUserCommand(userId);

            var result = await sender.Send(command, cancellationToken);

            return result.Match(
                _ => NoContent(),
                Problem);
        }

        // -----------------------------------------------------------------------
        // PATCH /api/v1/users/{userId}/deactivate
        // -----------------------------------------------------------------------

        [HttpPatch("{userId}/deactivate")]
        [Authorize(Policy = Permissions.Users.Deactivate)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType<ApiErrorResponse>(StatusCodes.Status404NotFound)]
        [ProducesResponseType<ApiErrorResponse>(StatusCodes.Status409Conflict)]
        [EndpointName("DeactivateUserV1")]
        [EndpointSummary("Deactivates a staff user account")]
        [EndpointDescription(
            "Sets the staff user's IsActive flag to false. The record is preserved for referential integrity " +
            "(Prescription.DoctorId, Dispense.PharmacistId). Returns 409 if already inactive. Admin accounts are protected.")]
        [MapToApiVersion("1.0")]
        public async Task<IActionResult> DeactivateUser(
            string userId,
            CancellationToken cancellationToken)
        {
            var command = new DeactivateUserCommand(userId);

            var result = await sender.Send(command, cancellationToken);

            return result.Match(
                _ => NoContent(),
                Problem);
        }
    }
}
