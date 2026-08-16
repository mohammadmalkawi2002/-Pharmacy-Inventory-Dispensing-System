using System;
using System.Collections.Generic;
using System.Text;

namespace PharmacyInventoryDispensingSystem.Application.Common.Interfaces
{
    public interface ITokenProvider
    {
        Task GenerateJwtTokenAsync(CancellationToken cancellationToken);
    }
}
