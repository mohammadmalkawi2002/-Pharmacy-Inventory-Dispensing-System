using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using PharmacyInventoryDispensingSystem.Application.Features.SecurityManager.Authorization;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace PharmacyInventoryDispensingSystem.Infrastructure.Identity.Jwt
{
    public sealed class JwtTokenProvider(
    IOptions<JwtOptions> options) : IJwtTokenProvider
    {
        private readonly JwtOptions _jwtOptions = options.Value;
        public TokenResult Generate(
            ApplicationUser user,
            IEnumerable<string> roles,
            IEnumerable<string> permissions)
        {
            // JWT Claims:
            var claimsIdentity = new ClaimsIdentity();

            claimsIdentity.AddClaim(
                new Claim(JwtRegisteredClaimNames.Sub, user.Id));

            claimsIdentity.AddClaim(
                new Claim(
                    JwtRegisteredClaimNames.Email,
                    user.Email ?? string.Empty));

            claimsIdentity.AddClaim(
                new Claim(ClaimTypes.NameIdentifier, user.Id));

            claimsIdentity.AddClaim(
                new Claim(
                    ClaimTypes.Email,
                    user.Email ?? string.Empty));

            //Custom claims for roles and permissions:

            foreach (var role in roles)
            {
                claimsIdentity.AddClaim(
                    new Claim(ClaimTypes.Role, role));
            }
            foreach (var permission in permissions)
            {
                claimsIdentity.AddClaim(
                    new Claim(
                        ApplicationClaimTypes.Permission,
                        permission));
            }

            var expiresAtUtc =
                DateTimeOffset.UtcNow.AddMinutes(
                    _jwtOptions.AccessTokenMinutes);

            var key = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(_jwtOptions.SigningKey));

            var credentials = new SigningCredentials(
                key,
                SecurityAlgorithms.HmacSha256);

            //JwtSecurityToken to add token options:
                 var token = new JwtSecurityToken(
                issuer: _jwtOptions.Issuer,
                audience: _jwtOptions.Audience,
                claims: claimsIdentity.Claims,
                notBefore: DateTime.UtcNow,
                expires: expiresAtUtc.UtcDateTime,
                signingCredentials: credentials);

            var accessToken =
                new JwtSecurityTokenHandler().WriteToken(token);

            return new TokenResult(
                accessToken,
                expiresAtUtc);
        }
    }
}
