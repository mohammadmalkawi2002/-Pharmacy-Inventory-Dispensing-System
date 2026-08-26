using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.OpenApi;
using Microsoft.OpenApi;

namespace PharmacyInventoryDispensingSystem.WebApi.OpenApi.Transformers
{
    internal sealed class BearerSecuritySchemeTransformer : IOpenApiDocumentTransformer,IOpenApiOperationTransformer
    {
        private const string SchemeId = JwtBearerDefaults.AuthenticationScheme;//="Bearer"

        public Task TransformAsync(
            OpenApiDocument document,
            OpenApiDocumentTransformerContext context,
            CancellationToken cancellationToken)
        {
            document.Components ??= new OpenApiComponents();

            document.Components.SecuritySchemes ??=
                new Dictionary<string, IOpenApiSecurityScheme>();

            document.Components.SecuritySchemes[SchemeId] =
                new OpenApiSecurityScheme
                {
                    Type = SecuritySchemeType.Http,
                    Scheme = "bearer",
                    BearerFormat = "JWT",
                    In = ParameterLocation.Header,
                    Name = "Authorization",
                    Description = "JWT Authorization header using Bearer scheme."
                };

            return Task.CompletedTask;
        }

        public Task TransformAsync(
            OpenApiOperation operation,
            OpenApiOperationTransformerContext context,
            CancellationToken cancellationToken)
        {
            var metadata = context.Description.ActionDescriptor.EndpointMetadata;

            var hasAuthorize = metadata
                .OfType<IAuthorizeData>()
                .Any();

            var hasAllowAnonymous = metadata
                .OfType<IAllowAnonymous>()
                .Any();

            if (!hasAuthorize || hasAllowAnonymous)
            {
                return Task.CompletedTask;
            }

            operation.Security ??= [];

            operation.Security.Add(
                new OpenApiSecurityRequirement
                {
                    [new OpenApiSecuritySchemeReference(SchemeId, context.Document)] = []
                });

            return Task.CompletedTask;
        }

        }
}
