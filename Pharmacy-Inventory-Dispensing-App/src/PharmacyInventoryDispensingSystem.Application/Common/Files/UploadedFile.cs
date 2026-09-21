using System;
using System.Collections.Generic;
using System.Text;

namespace PharmacyInventoryDispensingSystem.Application.Common.Files
{
    public sealed record UploadedFile(
     Stream Content,
     string FileName,
     string ContentType,
     long Length);
}
