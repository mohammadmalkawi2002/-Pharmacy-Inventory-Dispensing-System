using Microsoft.EntityFrameworkCore;
using PharmacyInventoryDispensingSystem.Application.Common.Interfaces;
using PharmacyInventoryDispensingSystem.Infrastructure.Persistence.Context;
using System;
using System.Collections.Generic;
using System.Text;

namespace PharmacyInventoryDispensingSystem.Infrastructure.Identity
{
    public sealed class UserLookupService(
    AppDbContext context) : IUserLookupService
    {
        public async Task<IReadOnlyDictionary<string, string>> GetUserNamesByIdsAsync(
            IReadOnlyCollection<string> userIds,
            CancellationToken cancellationToken = default)
        {
            return await context.Users
              .AsNoTracking()
              .Where(user => userIds.Contains(user.Id))
              .ToDictionaryAsync(
                  user => user.Id,
                  user => user.FirstName + " " + user.LastName,
                  cancellationToken);
        }
    }
}
