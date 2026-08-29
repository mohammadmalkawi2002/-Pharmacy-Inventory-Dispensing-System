using Asp.Versioning;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PharmacyInventoryDispensingSystem.Application.Features.SecurityManager.Authentication.Commands.ChangePassword;
using PharmacyInventoryDispensingSystem.Application.Features.SecurityManager.Authentication.Commands.ForgotPassword;
using PharmacyInventoryDispensingSystem.Application.Features.SecurityManager.Authentication.Commands.Login;
using PharmacyInventoryDispensingSystem.Application.Features.SecurityManager.Authentication.Commands.Logout;
using PharmacyInventoryDispensingSystem.Application.Features.SecurityManager.Authentication.Commands.Refresh;
using PharmacyInventoryDispensingSystem.Application.Features.SecurityManager.Authentication.Commands.Register;
using PharmacyInventoryDispensingSystem.Application.Features.SecurityManager.Authentication.Commands.ResetPassword;
using PharmacyInventoryDispensingSystem.Application.Features.SecurityManager.Authentication.DTOs;
using PharmacyInventoryDispensingSystem.Application.Features.SecurityManager.Authentication.Queries;
using PharmacyInventoryDispensingSystem.Application.Features.SecurityManager.Authorization;
using PharmacyInventoryDispensingSystem.WebApi.Contracts.ApiResponse;
using PharmacyInventoryDispensingSystem.WebApi.Contracts.Requests.Authentication;

namespace PharmacyInventoryDispensingSystem.WebApi.Controllers
{
    [ApiController]
    [Route("api/v{version:apiVersion}/auth")]
    [ApiVersion("1.0")]
    [Tags("Auths")]
    [Produces("application/json")]

    public sealed class AuthsController(ISender sender) : ApiController
    {

        [HttpPost("register")]
        [Authorize(Policy = PolicyNames.AdminOnly)]
        [Consumes("application/json")]
        [ProducesResponseType<AuthenticationResponse>(
    StatusCodes.Status201Created)]
        [ProducesResponseType<ApiErrorResponse>(StatusCodes.Status400BadRequest)]
        [ProducesResponseType<ApiErrorResponse>(StatusCodes.Status409Conflict)]
        [ProducesResponseType<ApiErrorResponse>(StatusCodes.Status500InternalServerError)]
        [EndpointName("RegisterV1")]
        [EndpointSummary("Registers a new user")]
        [EndpointDescription("Creates a new user account with the specified credentials and role.")]

        public async Task<IActionResult> Register([FromBody] RegisterRequest request,
                                                  CancellationToken cancellationToken)
        {
            var command = new RegisterUserCommand(
                request.email,
                request.password,
                request.FirstName,
                request.LastName,
                request.Role);

            var result = await sender.Send(command, cancellationToken);

            return result.Match(
                response => Created("", response)
                 , Problem);


        }


        [HttpPost("login")]
        [AllowAnonymous]
        [Consumes("application/json")]
        [ProducesResponseType<AuthenticationResponse>(
    StatusCodes.Status200OK)]
        [ProducesResponseType<ApiErrorResponse>(StatusCodes.Status400BadRequest)]
        [ProducesResponseType<ApiErrorResponse>(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType<ApiErrorResponse>(StatusCodes.Status500InternalServerError)]
        [EndpointName("LoginV1")]
        [EndpointSummary("Authenticates a user")]
        [EndpointDescription("Authenticates a user and returns an access token and refresh token.")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request,
                                               CancellationToken cancellationToken)
        {
            var command = new LoginCommand(request.Email, request.Password);

            var result = await sender.Send(command, cancellationToken);

            return result.Match(
          response => Ok(response),
          Problem);
        }


        [HttpPost("refresh")]
        [AllowAnonymous]
        [Consumes("application/json")]
        [ProducesResponseType<AuthenticationResponse>(
    StatusCodes.Status200OK)]
        [ProducesResponseType<ApiErrorResponse>(StatusCodes.Status400BadRequest)]
        [ProducesResponseType<ApiErrorResponse>(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType<ApiErrorResponse>(StatusCodes.Status500InternalServerError)]
        [EndpointName("RefreshTokenV1")]
        [EndpointSummary("Refreshes an access token")]
        [EndpointDescription("Uses a valid refresh token to issue a new access token and refresh token.")]
        public async Task<IActionResult> Refresh([FromBody] RefreshTokenRequest request,
                                                 CancellationToken cancellationToken)
        {
            var result = await sender.Send(new RefreshTokenCommand(request.RefreshToken), cancellationToken);
            return result.Match(
           response => Ok(response),
           Problem);
        }

        [HttpPost("logout")]
        [Authorize]
        [Consumes("application/json")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType<ApiErrorResponse>(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType<ApiErrorResponse>(StatusCodes.Status400BadRequest)]
        [ProducesResponseType<ApiErrorResponse>(StatusCodes.Status500InternalServerError)]
        [EndpointName("LogoutV1")]
        [EndpointSummary("Logs out the current user")]
        [EndpointDescription("Revokes the current refresh token and completes the logout operation.")]
        public async Task<IActionResult> Logout([FromBody] LogoutRequest request,
                                                CancellationToken cancellationToken)
        {
            var result = await sender.Send(new LogoutCommand(request.RefreshToken), cancellationToken);

            return result.Match(_ => NoContent(),
                        Problem);
        }

        [HttpPost("change-password")]
        [Authorize]
        [Consumes("application/json")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType<ApiErrorResponse>(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType<ApiErrorResponse>(StatusCodes.Status400BadRequest)]
        [ProducesResponseType<ApiErrorResponse>(StatusCodes.Status500InternalServerError)]
        [EndpointName("ChangePasswordV1")]
        [EndpointSummary("Changes the current user's password")]
        [EndpointDescription("Changes the authenticated user's password after validating the current password.")]

        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordRequest request,
                                                        CancellationToken cancellationToken)
        {
            var command = new ChangePasswordCommand(
                request.CurrentPassword,
                request.NewPassword);

            var result = await sender.Send(command, cancellationToken);

            return result.Match(_ => NoContent(),
                        Problem);

        }



        [HttpPost("forgot-password")]
        [AllowAnonymous]
        [Consumes("application/json")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType<ApiErrorResponse>(StatusCodes.Status400BadRequest)]
        [ProducesResponseType<ApiErrorResponse>(StatusCodes.Status500InternalServerError)]
        [EndpointName("ForgotPasswordV1")]
        [EndpointSummary("Requests a password reset")]
        [EndpointDescription("Sends password reset instructions to the specified email address when the account exists.")]

        public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordRequest request,
                                                        CancellationToken cancellationToken)
        {
            var command = new ForgotPasswordCommand(request.Email);

            var result= await sender.Send(command, cancellationToken);

            return result.Match(
                 _ => Ok(new
                 {
                 Success = true,
                Message = "If the email exists, password reset instructions were sent."
            }),
            Problem);

        }

        [HttpPost("reset-password")]
        [AllowAnonymous]
        [Consumes("application/json")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType<ApiErrorResponse>(StatusCodes.Status400BadRequest)]
        [ProducesResponseType<ApiErrorResponse>(StatusCodes.Status500InternalServerError)]
        [EndpointName("ResetPasswordV1")]
        [EndpointSummary("Resets a user's password")]
        [EndpointDescription("Resets the user's password using a valid password reset token.")]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordRequest request,
                                                       CancellationToken cancellationToken)
        {
            var command = new ResetPasswordCommand(
                request.Email,
                request.Token,
                request.NewPassword);

            var result=await sender.Send(command, cancellationToken);

            return (result.Match(_ => NoContent()
            ,Problem));
        }


        [HttpGet("me")]
        [Authorize]
        [ProducesResponseType<CurrentUserResponse>(
    StatusCodes.Status200OK)]
        [ProducesResponseType<ApiErrorResponse>(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType<ApiErrorResponse>(StatusCodes.Status404NotFound)]
        [ProducesResponseType<ApiErrorResponse>(StatusCodes.Status500InternalServerError)]
        [EndpointName("GetCurrentUserV1")]
        [EndpointSummary("Gets the current authenticated user")]
        [EndpointDescription("Returns the profile information of the currently authenticated user.")]
        public async Task<IActionResult> Me(
        CancellationToken cancellationToken)
        {
            var result = await sender.Send(new GetCurrentUserQuery(), cancellationToken);

            return result.Match(
           response => Ok(response),
           Problem);
        }


        }
}
