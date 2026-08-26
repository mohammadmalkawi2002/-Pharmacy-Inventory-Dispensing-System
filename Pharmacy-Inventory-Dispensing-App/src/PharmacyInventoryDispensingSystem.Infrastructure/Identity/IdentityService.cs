using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using PharmacyInventoryDispensingSystem.Application.Common.Interfaces;
using PharmacyInventoryDispensingSystem.Application.Features.SecurityManager.Authentication.DTOs;
using PharmacyInventoryDispensingSystem.Application.Features.SecurityManager.Authorization;
using PharmacyInventoryDispensingSystem.Domain.Common.Results;
using PharmacyInventoryDispensingSystem.Domain.Entities.Identity;
using PharmacyInventoryDispensingSystem.Infrastructure.Identity.Jwt;
using PharmacyInventoryDispensingSystem.Infrastructure.Identity.RefreshTokens;
using PharmacyInventoryDispensingSystem.Infrastructure.Persistence.Context;
using PharmacyInventoryDispensingSystem.Infrastructure.Services.Email;
using PharmacyInventoryDispensingSystem.Infrastructure.Services.Email.Templates;
using System;
using System.Collections.Generic;
using System.Text;

namespace PharmacyInventoryDispensingSystem.Infrastructure.Identity
{
    public class IdentityService(
        UserManager<ApplicationUser>  userManager,
         RoleManager<IdentityRole> roleManager,
         SignInManager<ApplicationUser> signInManager,
         IEmailService emailService,
        IJwtTokenProvider jwtTokenProvider,
        IRefreshTokenService refreshTokenService,
        ICurrentUser currentUser,
        ILogger<IdentityService> logger,
        IOptions<AuthenticationOptions> options,
        AppDbContext context) : IIdentityService
    {


        private readonly AuthenticationOptions authenticationOptions = options.Value;




        public async Task<Result<AuthenticationResponse>> RegisterAsync(
            string email,
            string password,
            string FirstName,
            string LastName,
            string Role,
            CancellationToken cancellationToken = default)
        {

            if (!RoleNames.All.Contains(Role)) 
            {
                logger.LogWarning(
                    "User registration rejected because role {Role} is invalid",
                    Role);

                return Error.Validation("Auth.Role.Invalid", "The selected role is invalid.");

            }

            //check the user is already exist:

            var existingUser = await userManager.FindByEmailAsync(email);

            if (existingUser is not null)
            {
                logger.LogWarning(
                    "User registration rejected because an account already exists. ExistingUserId: {UserId}",
                    existingUser.Id);

                return Error.Conflict("Auth.Email.Exists", "A user with this email already exists.");
            }


            var user = new ApplicationUser 
            {
                UserName = email,

                Email = email,
                 FirstName = FirstName,
                 LastName = LastName,
                 EmailConfirmed=true, //later when you apply email confirmation but false
                 IsActive = true,
                       
            
            };

            var createResult = await userManager.CreateAsync(user, password);

            if (!createResult.Succeeded) 
            {
                logger.LogWarning(
                    "Identity failed to create user. ErrorCount: {ErrorCount}, ErrorCodes: {ErrorCodes}",
                    createResult.Errors.Count(),
                    createResult.Errors.Select(x => x.Code).ToArray());


                return createResult.Errors
                       .Select(error=>Error.Validation(error.Code,error.Description))
                       .ToList();
            }


            // add the role:
         var roleResult=  await userManager.AddToRoleAsync(user, Role);

            if (!roleResult.Succeeded) 
            {
                logger.LogError(
                    "Failed to assign role {Role} to user {UserId}. ErrorCount: {ErrorCount}, ErrorCodes: {ErrorCodes}",
                    Role,
                    user.Id,
                    roleResult.Errors.Count(),
                    roleResult.Errors.Select(x => x.Code).ToArray());


                return roleResult.Errors
                    .Select(error =>
                        Error.Validation(error.Code, error.Description))
                        .ToList();
            }

            var response= await CreateAuthenticationResponseAsync(user, cancellationToken);

            logger.LogInformation(
                "User {UserId} registered successfully with role {Role}",
                user.Id,
                Role);

            return response;
        }



        public async Task<Result<AuthenticationResponse>> LoginAsync(
            string email,
            string password,
            CancellationToken cancellationToken = default)
        {
            //Find the user :

             var user=await userManager.FindByEmailAsync(email);

            //check if user Isactive?:
            if(user is null || !user.IsActive) 
            {
                logger.LogWarning(
                 "Login attempt rejected because the supplied credentials are invalid or user {UserId} is inactive",
                 user?.Id);


                return Error.Unauthorized(
                    "Auth.InvalidCredentials",
                    "Invalid email or password.");
            }

            //check password by signinManager to manage (FaildAttemps && Lockout):

            var signInResult = await signInManager.CheckPasswordSignInAsync(user, password, lockoutOnFailure: true);

            //Locked?
            if (signInResult.IsLockedOut) 
            {
                logger.LogWarning(
                    "Login attempt rejected because user {UserId} is locked out",
                    user.Id);

                return Error.Unauthorized(
                    "Auth.LockedOut",
                    "The account is temporarily locked.");
            }

            //Success?
            if (!signInResult.Succeeded)
            {
                return Error.Unauthorized(
                    "Auth.InvalidCredentials",
                    "Invalid email or password.");
            }

            user.LastLoginAtUtc = DateTimeOffset.UtcNow;

          var updateResult=  await userManager.UpdateAsync(user);

            if(!updateResult.Succeeded)
            {
                logger.LogError(
                    "Failed to update last login timestamp for user {UserId}. ErrorCount: {ErrorCount}, ErrorCodes: {ErrorCodes}",
                    user.Id,
                    updateResult.Errors.Count(),
                    updateResult.Errors.Select(x => x.Code).ToArray());

                return updateResult.Errors
                        .Select(error => Error.Failure(error.Code, error.Description))
                        .ToList();
            }

            var response= await CreateAuthenticationResponseAsync(user, cancellationToken);

            logger.LogInformation(
                "User {UserId} logged in successfully",
                user.Id);

            return response;
        }

       
        public async Task<Result<AuthenticationResponse>> RefreshAsync(
            string RefreshToken,
            CancellationToken cancellationToken = default)
        {
            //Check if RefreshToken input is exist in DB and not expired and not Revoked:
            var existingRefreshToken = await refreshTokenService.GetActiveTokenAsync(RefreshToken, cancellationToken);

            if(existingRefreshToken is null) 
            {
                logger.LogWarning(
                "Refresh token request rejected because the token is invalid, expired, or revoked");

                return Error.Unauthorized(
                    "Auth.RefreshToken.Invalid",
                    "Invalid refresh token.");
            }

            // 2. Get the user associated with the refresh token.
            var user =await userManager.FindByIdAsync(existingRefreshToken.UserId);


            if (user is null)
            {
                return Error.Unauthorized(
                    "Auth.User.NotFound",
                    "The user account was not found.");
            }


            if ( !user.IsActive)
            {

                logger.LogWarning(
                    "Refresh token request rejected because user {UserId} is inactive",
                    user.Id);

                return Error.Unauthorized(
                    "Auth.User.Inactive",
                    "The user account is inactive.");
            }


            // 3. Generate a new access token.
            var accessTokenResult = await jwtTokenProvider.GenerateAsync(user, cancellationToken);

            // 4. Create a new refresh token. => return newRefreshTokenResult.Entity.TokenHash &&newRefreshTokenResult.PlainTextToken

            var newRefreshTokenResult = await refreshTokenService.CreateAsync(user, cancellationToken);

            // 5. Revoke the old refresh token
            //    and link it to the replacement token.

            await refreshTokenService.RevokeAsync(
                existingRefreshToken,
                newRefreshTokenResult.Entity.TokenHash,
                cancellationToken);

            // 6. Persist both changes:
            //    - old token becomes revoked
            //    - new token is inserted

            await context.SaveChangesAsync(cancellationToken);

            logger.LogInformation(
                "Refresh token rotated successfully for user {UserId}",
                user.Id);

            // 7. Get user's roles and permissions for the response.

            var roles = await userManager.GetRolesAsync(user);
            var permissions = await GetPermissionsAsync(roles);

            return new AuthenticationResponse(
                user.Id,
                user.Email??string.Empty, 
                user.FirstName, 
                user.LastName, 
                roles.ToArray(),
                permissions.ToArray(),
                accessTokenResult.AccessToken,
                accessTokenResult.AccessTokenExpiresAtUtc,
                newRefreshTokenResult.PlainTextToken,
                newRefreshTokenResult.Entity.ExpiresAtUtc
                );

        }



        public async Task<Result<Success>> LogoutAsync(
            string RefreshToken,
            CancellationToken cancellationToken = default)
        {
            ////Check if RefreshToken input is exist in DB and not expired and not Revoked:
            var refreshToken = await refreshTokenService.GetActiveTokenAsync(RefreshToken, cancellationToken);
            if (refreshToken is not null)
            {
                //Revoke the refresh token:
                await refreshTokenService.RevokeAsync(refreshToken, cancellationToken: cancellationToken);
                await context.SaveChangesAsync(cancellationToken);

                logger.LogInformation(
                    "User {UserId} logged out and refresh token was revoked",
                    refreshToken.UserId);

            }

            return Result.Success;
        }



        public async Task<Result<CurrentUserResponse>> GetCurrentUserAsync(CancellationToken cancellationToken = default)
        {

            if (!currentUser.IsAuthenticated || currentUser.Id is null)
            {
                logger.LogWarning(
                 "Current user request rejected because the request is unauthenticated");


                return Error.Unauthorized("Auth.Unauthorized", "Authentication is required.");

            }

            var user = await userManager.FindByIdAsync(currentUser.Id);

            if (user is null)
            {

                logger.LogWarning("Authenticated identity references a user that does not exist. UserId: {UserId}",
                    currentUser.Id);

                return Error.Unauthorized("Auth.User.NotFound", "Authentication is required.");


            }

            var roles = await userManager.GetRolesAsync(user);
            var permissions = await GetPermissionsAsync(roles);

            return new CurrentUserResponse(user.Id,
                user.Email ?? string.Empty,
                user.FirstName,
                user.LastName,
                roles.ToArray(),
                permissions.ToArray());
        }


        public async Task<Result<Success>> ChangePasswordAsync(string CurrentPassword, string NewPassword, CancellationToken cancellationToken = default)
        {
            // 1. Make sure the request comes from an authenticated user.
                if(!currentUser.IsAuthenticated || currentUser.Id is null) 
                {
                logger.LogWarning(
                         "Password change rejected because the request is unauthenticated");
                return Error.Unauthorized(
                    "Auth.Unauthorized",
                    "Authentication is required.");
                }

            // 2. Load the current user from Identity.

                var user= await userManager.FindByIdAsync(currentUser.Id);

            if (user is null || !user.IsActive)
            {

                logger.LogWarning("Password change rejected because user {UserId} does not exist",
                    currentUser.Id);

                return Error.Unauthorized(
                    "Auth.Unauthorized",
                    "Authentication is required.");
            }

            //      verify the current password  and validate/update the new password using ASP Identity:

         var result=   await userManager.ChangePasswordAsync(user, CurrentPassword, NewPassword);


            if (!result.Succeeded)
            {
                logger.LogWarning(
                    "Password change failed for user {UserId}. ErrorCount: {ErrorCount}, ErrorCodes: {ErrorCodes}",
                    user.Id,
                    result.Errors.Count(),
                    result.Errors.Select(x => x.Code).ToArray());


                    return result.Errors
                    .Select(error =>
                        Error.Validation(
                            error.Code,
                            error.Description))
                    .ToList();
            }

            logger.LogInformation(
                "Password changed successfully for user {UserId}",
                user.Id);

            return Result.Success;

        }

        public async Task<Result<Success>> ForgotPasswordAsync(string Email, CancellationToken cancellationToken = default)
        {
            var user = await userManager.FindByEmailAsync(Email);

            if(user is null || !user.IsActive) 
            { 
                return Result.Success;
            }


            var token = await userManager.GeneratePasswordResetTokenAsync(user);

            var encodedEmail = Uri.EscapeDataString(Email);
            var encodedToken = Uri.EscapeDataString(token);


            var resetUrl = 
                    $"{authenticationOptions.FrontendUrl}/reset-password?email={encodedEmail}&token={encodedToken}";

            var htmlMessage = PasswordResetEmailTemplate.Build(resetUrl);

            await emailService.SendEmailAsync(Email,"Reset Your Password",htmlMessage, cancellationToken);

            logger.LogInformation(
                "Password reset email sent for user {UserId}",
                user.Id);

            return Result.Success;
        }


     

        public async Task<Result<Success>> ResetPasswordAsync(
            string Email,
            string Token,
            string NewPassword,
            CancellationToken cancellationToken = default)
        {

            var user = await userManager.FindByEmailAsync(Email);

            if (user is null || !user.IsActive) 
            {
                logger.LogWarning(
                 "Password reset request rejected because the request is invalid");
                return Error.Validation(
                    "Auth.ResetPassword.Invalid",
                    "Invalid password reset request.");

            }


            var result = await userManager.ResetPasswordAsync(user, Token, NewPassword);

            if (!result.Succeeded) 
            {

                logger.LogWarning(
                    "Password reset failed for user {UserId}. ErrorCount: {ErrorCount}, ErrorCodes: {ErrorCodes}",
                    user.Id,
                    result.Errors.Count(),
                    result.Errors.Select(x => x.Code).ToArray());



                return result.Errors
                .Select(error =>
                Error.Validation(
                error.Code,
                error.Description))
                .ToList();

            }

            logger.LogInformation(
                "Password reset successfully for user {UserId}",
                user.Id);


            return Result.Success;
        }


        private async Task<AuthenticationResponse> CreateAuthenticationResponseAsync(
            ApplicationUser user,
            CancellationToken cancellationToken) 
        {
            var accessToken = await jwtTokenProvider.GenerateAsync(user, cancellationToken);

          var refreshTokenResult =  await refreshTokenService.CreateAsync(user, cancellationToken);
            var roles = await userManager.GetRolesAsync(user);
            var permissions = await GetPermissionsAsync(roles);

            await context.SaveChangesAsync(cancellationToken);


            return new AuthenticationResponse(
                user.Id,
                user.Email??string.Empty,
                user.FirstName,
                user.LastName,
                roles.ToArray(),
                permissions.ToArray(),
                accessToken.AccessToken,
                accessToken.AccessTokenExpiresAtUtc,
                refreshTokenResult.PlainTextToken, refreshTokenResult.Entity.ExpiresAtUtc);



        }


        private async Task<HashSet<string>> GetPermissionsAsync(
    IEnumerable<string> roleNames)
        {
            var permissions = new HashSet<string>();

            foreach (var roleName in roleNames)
            {
                var role = await roleManager.FindByNameAsync(roleName);

                if (role is null)
                {
                    continue;
                }

                var claims = await roleManager.GetClaimsAsync(role);

                foreach (var claim in claims)
                {
                    if (claim.Type == ApplicationClaimTypes.Permission)
                    {
                        permissions.Add(claim.Value);
                    }
                }
            }

            return permissions;
        }

    }
}
