using Asp.Versioning;
using Asp.Versioning.OpenApi;
using PharmacyInventoryDispensingSystem.WebApi.Contracts.ApiResponse;
using PharmacyInventoryDispensingSystem.WebApi.Middlewares;
using PharmacyInventoryDispensingSystem.WebApi.OpenApi.Transformers;
using PharmacyInventoryDispensingSystem.WebApi.RateLimiting;
using System.Security.Claims;
using System.Text.Json.Serialization;
using System.Threading.RateLimiting;

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
               .AddValidation()
               .AddConfiguredCors(configuration)
               .AddAppRateLimiting();


          


            return services;
        }



        public static IServiceCollection AddControllerWithJsonConfiguration(this IServiceCollection services)
        {
            services.AddControllers()
                .AddJsonOptions(options => 
                {
                    options.JsonSerializerOptions.DefaultIgnoreCondition =
                   JsonIgnoreCondition.WhenWritingNull;

                    options.JsonSerializerOptions.Converters.Add(
                        new JsonStringEnumConverter(allowIntegerValues:false));

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

                options.Document
    .AddSchemaTransformer<EnumSchemaTransformer>();


            });

            return services;
        }

        public static IServiceCollection AddAppRateLimiting(this IServiceCollection services)
        {
            services.AddRateLimiter(options =>
            {
                options.RejectionStatusCode =
                    StatusCodes.Status429TooManyRequests;


                options.OnRejected = async (
                    context,
                    cancellationToken) =>
                {
                    const string message =
                        "Too many requests. Please try again later.";

                    context.HttpContext.Response.StatusCode =
                        StatusCodes.Status429TooManyRequests;

                    var response = new
                    {
                        success = false,
                        message,
                        errors = new Dictionary<string, string[]>
                        {
                            ["RateLimit.Exceeded"] = [message]
                        },
                        traceId =
                            context.HttpContext.TraceIdentifier
                    };

                    await context.HttpContext.Response.WriteAsJsonAsync(
                        response,
                        cancellationToken);
                };

                // Login, Refresh, Forgot Password, Reset Password
                options.AddPolicy(
                    RateLimitPolicyNames.AnonymousAuth,
                    httpContext =>
                    {
                        string ipAddress =
                            httpContext.Connection
                                .RemoteIpAddress?
                                .ToString()
                            ?? "unknown";

                        string partitionKey =
                            $"{ipAddress}:{httpContext.Request.Path}";

                        return RateLimitPartition
                            .GetSlidingWindowLimiter(
                                partitionKey,
                                _ => new SlidingWindowRateLimiterOptions
                                {
                                    PermitLimit = 5,

                                    Window =
                                        TimeSpan.FromMinutes(1),

                                    SegmentsPerWindow = 6,

                                    QueueLimit = 0,

                                    QueueProcessingOrder =
                                        QueueProcessingOrder.OldestFirst,

                                    AutoReplenishment = true
                                });
                    });

                // Register, Logout, Change Password, Me
                options.AddPolicy(
                    RateLimitPolicyNames.AuthenticatedAuth,
                    httpContext =>
                    {
                        string clientId =
                            httpContext.User.FindFirstValue(
                                ClaimTypes.NameIdentifier)
                            ?? httpContext.User.FindFirstValue("sub")
                            ?? httpContext.Connection
                                .RemoteIpAddress?
                                .ToString()
                            ?? "unknown";

                        string partitionKey =
                            $"{clientId}:{httpContext.Request.Path}";

                        return RateLimitPartition
                            .GetSlidingWindowLimiter(
                                partitionKey,
                                _ => new SlidingWindowRateLimiterOptions
                                {
                                    PermitLimit = 10,

                                    Window =
                                        TimeSpan.FromMinutes(1),

                                    SegmentsPerWindow = 6,

                                    QueueLimit = 0,

                                    QueueProcessingOrder =
                                        QueueProcessingOrder.OldestFirst,

                                    AutoReplenishment = true
                                });
                    });  
            });

            return services;
        }

        public static IServiceCollection AddExceptionHandling(this IServiceCollection services) 
        {
            services.AddExceptionHandler<GlobalExceptionHandler>();
            services.AddProblemDetails();


            return services;
        }


        public static IServiceCollection AddConfiguredCors(this IServiceCollection services, IConfiguration configuration)
        {
            var frontendUrl = configuration["Authentication:FrontendUrl"]
                ?? throw new InvalidOperationException("Frontend URL is not configured.");


            services.AddCors(options =>
            options.AddPolicy(
                "AllowFrontend",
                policy => policy
                    .WithOrigins(frontendUrl)
                    .AllowAnyHeader()
                    .AllowAnyMethod()
                    .AllowCredentials()));

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
            app.UseCors("AllowFrontend");


            // 6. Authentication (must come before authorization)
            app.UseAuthentication();
          // 7. Rate limiting (after authentication to get userId and claims)
            app.UseRateLimiter();
          // 8. Authorization (must come after authentication)
           app.UseAuthorization();
          // 9. Output caching (after auth to cache based on user context)


            return app;
        }

    }
}
