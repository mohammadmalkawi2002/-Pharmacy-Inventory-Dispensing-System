using System;
using System.Collections.Generic;
using System.Text;

namespace PharmacyInventoryDispensingSystem.Application.Common.Interfaces
{
    public interface IUserLookupService
    {
        Task<IReadOnlyDictionary<string, string>> GetUserNamesByIdsAsync(
            IReadOnlyCollection<string> userIds,
            CancellationToken cancellationToken = default);
    }
}
