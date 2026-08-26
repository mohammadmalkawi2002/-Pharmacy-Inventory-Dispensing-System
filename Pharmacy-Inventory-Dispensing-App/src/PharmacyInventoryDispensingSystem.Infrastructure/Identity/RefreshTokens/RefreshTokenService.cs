using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using PharmacyInventoryDispensingSystem.Domain.Entities.Identity;
using PharmacyInventoryDispensingSystem.Infrastructure.Identity.Jwt;
using PharmacyInventoryDispensingSystem.Infrastructure.Persistence.Context;
using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace PharmacyInventoryDispensingSystem.Infrastructure.Identity.RefreshTokens
{
    public class RefreshTokenService(
        AppDbContext context,
        IOptions<JwtOptions> options) : IRefreshTokenService
    {

        private readonly JwtOptions _options=options.Value;


        /// <summary>
        /// Generate plainText refresh token(string)
        /// </summary>
        /// <returns></returns>
        public string GenerateRefreshToken()
        {
            var bytes = RandomNumberGenerator.GetBytes(64);
            return Convert.ToBase64String(bytes);
        }

        public async Task<RefreshToken?> GetActiveTokenAsync(string token, CancellationToken cancellationToken = default)
        {
            var tokenhash = HashToken(token);


         return await   context.RefreshTokens.FirstOrDefaultAsync
                (
                    x=>x.TokenHash== tokenhash&&
                    x.RevokedAtUtc==null&&
                    x.ExpiresAtUtc> DateTimeOffset.UtcNow,
                    cancellationToken
                );

        }

        public DateTimeOffset GetExpiration()
        {
            return DateTimeOffset.UtcNow.
                    AddDays(_options.RefreshTokenDays);
        }

        public string HashToken(string token)
        {
            var bytes = SHA256.HashData(
                            Encoding.UTF8.GetBytes(token));

            return Convert.ToHexString(bytes);

        }



        /// <summary>
        /// Create new  Refresh Token and store it in DB
        /// </summary>
        /// <param name="user"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>

        public async Task<RefreshTokenResult> CreateAsync(ApplicationUser user, CancellationToken cancellationToken = default)
        {
            //This is actual token as A PlainText must return to the client:
            var plainTextToken = GenerateRefreshToken();

            var refreshToken = new RefreshToken
            {
                    UserId = user.Id,
                 TokenHash=HashToken(plainTextToken),
                  ExpiresAtUtc = GetExpiration(),
                  
            };

            await context.RefreshTokens.AddAsync(refreshToken, cancellationToken);

            return new RefreshTokenResult(refreshToken, plainTextToken);
          
        }

      
        public Task RevokeAsync(RefreshToken refreshToken, string? replacedByTokenHash = null, CancellationToken cancellationToken = default)
        {
            refreshToken.RevokedAtUtc = DateTimeOffset.UtcNow;
            refreshToken.ReplacedByTokenHash = replacedByTokenHash;

            return Task.CompletedTask;
        }
    }
}
