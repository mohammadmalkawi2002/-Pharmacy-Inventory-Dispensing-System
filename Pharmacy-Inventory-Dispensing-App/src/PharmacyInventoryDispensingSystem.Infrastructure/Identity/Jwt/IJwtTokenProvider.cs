using System;
using System.Collections.Generic;
using System.Text;

namespace PharmacyInventoryDispensingSystem.Infrastructure.Identity.Jwt
{
    public interface IJwtTokenProvider
    {
       TokenResult Generate(ApplicationUser user,IEnumerable<string> roles,IEnumerable<string> permissions);
    }
}
