using PharmacyInventoryDispensingSystem.Domain.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace PharmacyInventoryDispensingSystem.Domain.Entities.FileSystem
{
    public sealed class FileImage:AuditableEntity
    {
        public string OriginalFileName { get; set; } = null!;

        public string StoredFileName { get; set; } = null!;

        public string ContentType { get; set; } = null!;

        public string Extension { get; set; } = null!;

        public long FileSize { get; set; }
    }
}
