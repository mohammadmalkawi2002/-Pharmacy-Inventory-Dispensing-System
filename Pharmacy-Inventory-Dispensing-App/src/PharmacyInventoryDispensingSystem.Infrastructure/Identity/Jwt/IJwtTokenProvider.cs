using System;
using System.Collections.Generic;
using System.Text;

namespace PharmacyInventoryDispensingSystem.Infrastructure.Identity.Jwt
{
    public interface IJwtTokenProvider
    {
        Task<TokenResult> GenerateAsync(ApplicationUser user,CancellationToken cancellationToken=default);
    }
}
