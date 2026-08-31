using Microsoft.AspNetCore.OpenApi;
using Microsoft.OpenApi;
using System.Text.Json.Nodes;

namespace PharmacyInventoryDispensingSystem.WebApi.OpenApi.Transformers
{
    public sealed class EnumSchemaTransformer : IOpenApiSchemaTransformer
    {
        public Task TransformAsync(
            OpenApiSchema schema,
            OpenApiSchemaTransformerContext context,
            CancellationToken cancellationToken)
        {
            var type = context.JsonTypeInfo.Type;

            var enumType = Nullable.GetUnderlyingType(type) ?? type;

            if (!enumType.IsEnum)
                return Task.CompletedTask;

            schema.Type = JsonSchemaType.String;
            schema.Format = null;

            schema.Enum = Enum.GetNames(enumType)
                .Select(name => (JsonNode)JsonValue.Create(name)!)
                .ToList();

            return Task.CompletedTask;
        }
    }
}
