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
        RefreshTokenResult Create(ApplicationUser user);
        void Revoke(RefreshToken refreshToken,
                         string? replacedByTokenHash = null);
    }
}
