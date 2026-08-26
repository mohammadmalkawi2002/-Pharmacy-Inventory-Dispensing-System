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
    UserManager<ApplicationUser> userManager,
    RoleManager<IdentityRole> roleManager,
    IOptions<JwtOptions> options) : IJwtTokenProvider
    {

        private readonly JwtOptions _jwtOptions = options.Value;
        public async Task<TokenResult> GenerateAsync(ApplicationUser user, CancellationToken cancellationToken = default)
        {
            // 1] Get All roles name that the specified user belongs to ex: admin:
            var roles = await userManager.GetRolesAsync(user);

            var permissions = new HashSet<string>();

            foreach (var roleName in roles) 
            { 
                cancellationToken.ThrowIfCancellationRequested();

                var role= await roleManager.FindByNameAsync(roleName);

                if(role is null) 
                {
                    continue;
                }

                //2] GetClaimsAsync(Admin)

                var claims = await roleManager.GetClaimsAsync(role);

                //3] Permissions:
                foreach (var claim in claims) 
                {
                    if (claim.Type == ApplicationClaimTypes.Permission) 
                    {
                        permissions.Add(claim.Value);
                    
                    }
                }
            }


            //4] JWT Claims:
            var claimsIdentity = new ClaimsIdentity();

            claimsIdentity.AddClaim(
                        new Claim(JwtRegisteredClaimNames.Sub,user.Id));
            claimsIdentity.AddClaim(
                   new Claim(JwtRegisteredClaimNames.Email,user.Email??string.Empty));

            claimsIdentity.AddClaim(
                new Claim(ClaimTypes.NameIdentifier,user.Id));

            claimsIdentity.AddClaim(
                new Claim(ClaimTypes.Email,user.Email??string.Empty));

            //Custom calims for roles and permissionS:

            foreach (var role in roles) 
            {
                claimsIdentity.AddClaim(
                        new Claim(ClaimTypes.Role,role));            
            }

            foreach(var permission in permissions) 
            {
                claimsIdentity.AddClaim(
                        new Claim(
                            ApplicationClaimTypes.Permission,permission
                            )
                        );
            
            }

            var expiresAtUtc = DateTimeOffset.UtcNow.AddMinutes(_jwtOptions.AccessTokenMinutes);
            var key=new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtOptions.SigningKey));
            var credentials=new SigningCredentials(key,SecurityAlgorithms.HmacSha256);


            //JwtSecurityToken to add token options:

            var token = new JwtSecurityToken(
                issuer: _jwtOptions.Issuer,
                audience: _jwtOptions.Audience,
                claims: claimsIdentity.Claims,
                notBefore: DateTime.UtcNow,
                expires: expiresAtUtc.UtcDateTime,
                signingCredentials: credentials);

            
            var accessToken=new JwtSecurityTokenHandler().WriteToken(token);



            return new TokenResult(accessToken,expiresAtUtc);
          
            
        }
    }
}
