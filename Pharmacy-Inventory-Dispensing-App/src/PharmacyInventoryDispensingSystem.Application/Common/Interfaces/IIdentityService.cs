using PharmacyInventoryDispensingSystem.Application.Features.SecurityManager.Authentication.DTOs;
using PharmacyInventoryDispensingSystem.Domain.Common.Results;
using System;
using System.Collections.Generic;
using System.Text;

namespace PharmacyInventoryDispensingSystem.Application.Common.Interfaces
{
    public interface IIdentityService
    {
        Task<Result<AuthenticationResponse>> RegisterAsync(string email, string password, string FirstName,
                string LastName,string Role,CancellationToken cancellationToken = default);
        Task<Result<AuthenticationResponse>> LoginAsync(string email, string password, CancellationToken cancellationToken = default);

        Task<Result<AuthenticationResponse>> RefreshAsync(string RefreshToken, CancellationToken cancellationToken = default);

        Task<Result<Success>> LogoutAsync(string RefreshToken,CancellationToken cancellationToken = default);

        Task<Result<Success>> ChangePasswordAsync(string CurrentPassword,string NewPassword, CancellationToken cancellationToken = default);

        Task<Result<Success>> ForgotPasswordAsync(string Email, CancellationToken cancellationToken = default);

        Task<Result<Success>> ResetPasswordAsync(string Email,
                                                string Token,
                                                 string NewPassword, 
                                                 CancellationToken cancellationToken = default
                                                );
        
        Task<Result<CurrentUserResponse>> GetCurrentUserAsync(CancellationToken cancellationToken = default);
    }
}
