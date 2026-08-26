using System;
using System.Collections.Generic;
using System.Text;

namespace PharmacyInventoryDispensingSystem.Domain.Entities.Identity
{
    public sealed class RefreshToken
    {
        public Guid Id { get; set; } = Guid.CreateVersion7();

        public string UserId { get; set; } = string.Empty;

        public string TokenHash { get; set; } = string.Empty;

        public DateTimeOffset CreatedAtUtc { get; set; } = DateTimeOffset.UtcNow;

        public DateTimeOffset ExpiresAtUtc { get; set; }

        public DateTimeOffset? RevokedAtUtc { get; set; }

        public string? ReplacedByTokenHash { get; set; }

        

        public bool IsExpired =>
            DateTimeOffset.UtcNow >= ExpiresAtUtc;

        public bool IsRevoked =>
            RevokedAtUtc.HasValue;

        public bool IsActive =>
            !IsExpired && !IsRevoked;
    }
}
