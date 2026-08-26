using Microsoft.AspNetCore.OpenApi;
using Microsoft.OpenApi;
using PharmacyInventoryDispensingSystem.WebApi.Contracts.ApiResponse;
using System.Text.Json.Nodes;

namespace PharmacyInventoryDispensingSystem.WebApi.OpenApi.Transformers
{
    public sealed class ApiErrorResponseSchemaTransformer : IOpenApiSchemaTransformer
    {
        public Task TransformAsync(
            OpenApiSchema schema,
            OpenApiSchemaTransformerContext context,
            CancellationToken cancellationToken)
        {
            if (context.JsonTypeInfo.Type == typeof(ApiErrorResponse)) 
            {
                schema.Example = JsonNode.Parse("""
                  {
              "success": false,
              "message": "string",
              "errors": {},
              "traceId": "string"
                 }
              """);
            }
            return Task.CompletedTask;

        }
    }
}
