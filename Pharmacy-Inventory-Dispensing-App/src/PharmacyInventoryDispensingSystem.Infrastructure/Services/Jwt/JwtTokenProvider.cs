using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using PharmacyInventoryDispensingSystem.Application.Common.Interfaces;

namespace PharmacyInventoryDispensingSystem.Infrastructure.Services.Jwt;

internal sealed class JwtTokenProvider(IOptions<JwtSettings> options) : ITokenProvider
{
    public string GenerateAccessToken(string userId, string email, IEnumerable<string> roles)
    {
        var settings = options.Value;

        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, userId),
            new(JwtRegisteredClaimNames.Email, email),
            new(ClaimTypes.NameIdentifier, userId),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        claims.AddRange(roles.Select(role => new Claim(ClaimTypes.Role, role)));

        var descriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = DateTime.UtcNow.AddMinutes(settings.ExpiryMinutes),
            Issuer = settings.Issuer,
            Audience = settings.Audience,
            SigningCredentials = new SigningCredentials(
                new SymmetricSecurityKey(Encoding.UTF8.GetBytes(settings.Key)),
                SecurityAlgorithms.HmacSha256)
        };

        return new JsonWebTokenHandler().CreateToken(descriptor);
    }
}

