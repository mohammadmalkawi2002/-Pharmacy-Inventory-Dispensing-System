using Microsoft.AspNetCore.OpenApi;
using Microsoft.OpenApi;

namespace PharmacyInventoryDispensingSystem.WebApi.OpenApi.Transformers
{
    internal sealed class VersionInfoTransformer : IOpenApiDocumentTransformer
    {
        public Task TransformAsync(
            OpenApiDocument document,
            OpenApiDocumentTransformerContext context,
            CancellationToken cancellationToken)
        {

            var version = context.DocumentName;
            document.Info.Version = version;
            document.Info.Title = $"PharmacyInventoryDispensingSystem API {version}";

            return Task.CompletedTask;
        }
    }
}
