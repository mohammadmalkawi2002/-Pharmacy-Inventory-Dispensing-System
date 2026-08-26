using Asp.Versioning;
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
               .AddApiDocumentation()
               .AddExceptionHandling()
               .AddControllerWithJsonConfiguration()
               .AddValidation();
              

            return services;
        }



        public static IServiceCollection AddControllerWithJsonConfiguration(this IServiceCollection services)
        {
            services.AddControllers().AddJsonOptions(options => options
                .JsonSerializerOptions
                .DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull);

            return services;
        }


        public static IServiceCollection AddCustomApiVersioning(this IServiceCollection services)
        {
            services.AddApiVersioning(options =>
            {
                options.DefaultApiVersion = new ApiVersion(1);
                options.AssumeDefaultVersionWhenUnspecified = true;
                options.ReportApiVersions = true;
                options.ApiVersionReader = new UrlSegmentApiVersionReader();
            }).AddMvc()
            .AddApiExplorer(options =>
            {
                options.GroupNameFormat = "'v'VVV";
                options.SubstituteApiVersionInUrl = true;
            });

            return services;
        }

        public static IServiceCollection AddApiDocumentation(this IServiceCollection services) 
        {
            string[] versions = ["v1"];


            foreach (var version in versions) 
            {
                services.AddOpenApi(version, options => 
                {
                    // Versioning config
                    options.AddDocumentTransformer<VersionInfoTransformer>();
                    // Security Scheme config:

                    options.AddDocumentTransformer<BearerSecuritySchemeTransformer>();

                    // Security Operation config
                    options.AddOperationTransformer<BearerSecuritySchemeTransformer>();

                    options.AddSchemaTransformer<ApiErrorResponseSchemaTransformer>();



                });
            
            }

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
            app.UseStatusCodePages();

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
