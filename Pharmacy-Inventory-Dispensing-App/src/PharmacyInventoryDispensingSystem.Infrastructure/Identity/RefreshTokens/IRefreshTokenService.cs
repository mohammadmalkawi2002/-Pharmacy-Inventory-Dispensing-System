using PharmacyInventoryDispensingSystem.Domain.Entities.Identity;
using System;
using System.Collections.Generic;
using System.Text;

namespace PharmacyInventoryDispensingSystem.Infrastructure.Identity.RefreshTokens
{
    public interface IRefreshTokenService
    {
        string GenerateRefreshToken();
        string HashToken(string token);
        DateTimeOffset GetExpiration();

        Task<RefreshToken?> GetActiveTokenAsync(string token,
                                                CancellationToken cancellationToken = default);

        Task<RefreshTokenResult> CreateAsync(ApplicationUser user,
                                       CancellationToken cancellationToken = default);


        Task RevokeAsync(RefreshToken refreshToken,
                         string? replacedByTokenHash = null,
                         CancellationToken cancellationToken = default);
    }
}
