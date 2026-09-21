using System;
using System.Collections.Generic;
using System.Text;

namespace PharmacyInventoryDispensingSystem.Infrastructure.Services.FileImageManager
{
    public class FileImageSettings
    {
        public string PathFolder { get; set; } = string.Empty;
        public int MaxFileSizeInMB { get; set; }
    }

}
