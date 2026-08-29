using Asp.Versioning;
using Asp.Versioning.OpenApi;
using PharmacyInventoryDispensingSystem.WebApi.Contracts.ApiResponse;
using PharmacyInventoryDispensingSystem.WebApi.Middlewares;
using PharmacyInventoryDispensingSystem.WebApi.OpenApi.Transformers;
using System.Text.Json.Serialization;

namespace PharmacyInventoryDispensingSystem.WebApi
{
    public static class DependencyInjection
    {

        public static IServiceCollection AddPresentation(this IServiceCollection services, IConfiguration configuration)
        {

            services
               .AddCustomApiVersioning()
               .AddExceptionHandling()
               .AddControllerWithJsonConfiguration()
               .AddValidation();
              

            return services;
        }



        public static IServiceCollection AddControllerWithJsonConfiguration(this IServiceCollection services)
        {
            services.AddControllers().AddJsonOptions(options => {
                options
                .JsonSerializerOptions
                .DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;

                options.JsonSerializerOptions.Converters.Add(
                    new JsonStringEnumConverter() );

            });

            return services;
        }


        public static IServiceCollection AddCustomApiVersioning(this IServiceCollection services)
        {
            services.AddApiVersioning(options =>
            {
                options.DefaultApiVersion = new ApiVersion(1);
                options.ReportApiVersions = true;
                options.ApiVersionReader = new UrlSegmentApiVersionReader();
            })
             .AddMvc()
             .AddApiExplorer(options =>
             {
              options.GroupNameFormat = "'v'VVV";
             options.SubstituteApiVersionInUrl = true;
             })
          .AddOpenApi(options =>
            {
            options.Document
            .AddDocumentTransformer<VersionInfoTransformer>();

              options.Document
            .AddDocumentTransformer<BearerSecuritySchemeTransformer>();

             options.Document
            .AddOperationTransformer<BearerSecuritySchemeTransformer>();

             options.Document
            .AddSchemaTransformer<ApiErrorResponseSchemaTransformer>();

                options .Document.AddSchemaTransformer<AuthenticationExamplesSchemaTransformer>();

            
             });

            return services;
        }


        public static IServiceCollection AddExceptionHandling(this IServiceCollection services) 
        {
            services.AddExceptionHandler<GlobalExceptionHandler>();
            services.AddProblemDetails();


            return services;
        }

        public static IApplicationBuilder UseCoreMiddlewares(this IApplicationBuilder app, IConfiguration configuration)
        {
            // 1. Exception handling should be FIRST to catch all errors
            app.UseExceptionHandler();

            // 2. Status code pages for handling HTTP status codes
            app.UseStatusCodePages(async statusCodeContext =>
            {
                var httpContext = statusCodeContext.HttpContext;
                var statusCode = httpContext.Response.StatusCode;

                var (errorCode, message) = statusCode switch
                {
                    StatusCodes.Status400BadRequest =>
                        ("Request.BadRequest", "The request is invalid."),

                    StatusCodes.Status401Unauthorized =>
                        ("Authentication.Unauthorized", "Authentication is required."),

                    StatusCodes.Status403Forbidden =>
                        (
                            "Authorization.Forbidden",
                            "You do not have permission to access this resource."
                        ),

                    StatusCodes.Status404NotFound =>
                        (
                            "Request.NotFound",
                            "The requested resource was not found."
                        ),

                    StatusCodes.Status405MethodNotAllowed =>
                        (
                            "Request.MethodNotAllowed",
                            "The requested HTTP method is not allowed."
                        ),

                    _ =>
                        (
                            "Request.Failed",
                            "The request could not be completed."
                        )
                };

                var errors = new Dictionary<string, string[]>
                {
                    [errorCode] = [message]
                };

                var response = new ApiErrorResponse(
                    Success: false,
                    Message: message,
                    Errors: errors,
                    TraceId: httpContext.TraceIdentifier);

                await httpContext.Response.WriteAsJsonAsync(
                    response,
                    httpContext.RequestAborted);
            });

            // 3. HTTPS redirection (before any other middleware that might generate URLs)
            app.UseHttpsRedirection();
            // 4. Serilog request logging (early to log all requests) later i do not understand it

            // 5. CORS (before authentication/authorization)


            // 6. Rate limiting (before authentication to protect auth endpoints)


            // 7. Authentication (must come before authorization)
            app.UseAuthentication();

            // 8. Authorization (must come after authentication)
            app.UseAuthorization();
            // 9. Output caching (after auth to cache based on user context)


            return app;
        }

    }
}
