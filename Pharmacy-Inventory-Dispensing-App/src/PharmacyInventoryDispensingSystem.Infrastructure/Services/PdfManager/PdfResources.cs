using System;
using System.Collections.Generic;
using System.Text;

namespace PharmacyInventoryDispensingSystem.Infrastructure.Services.PdfManager
{
    internal static class PdfResources
    {
        public static readonly byte[] Logo = LoadEmbeddedResource(
            "PharmacyInventoryDispensingSystem.Infrastructure.Resources.logo.png");

        private static byte[] LoadEmbeddedResource(string resourceName)
        {
            var assembly = typeof(PdfResources).Assembly;

            using var stream = assembly.GetManifestResourceStream(resourceName)
                ?? throw new InvalidOperationException(
                    $"Embedded resource '{resourceName}' was not found.");

            using var memoryStream = new MemoryStream();
            stream.CopyTo(memoryStream);

            return memoryStream.ToArray();
        }
    }
}
