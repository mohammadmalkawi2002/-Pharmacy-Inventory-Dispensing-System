using Microsoft.AspNetCore.Diagnostics;
using PharmacyInventoryDispensingSystem.WebApi.Contracts.ApiResponse;

namespace PharmacyInventoryDispensingSystem.WebApi.Middlewares
{
    public class GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger) : IExceptionHandler
    {
        public async ValueTask<bool> TryHandleAsync(
            HttpContext httpContext,
            Exception exception,
            CancellationToken cancellationToken)
        {
            logger.LogError(exception, "An unhandled exception occurred.");

            httpContext.Response.StatusCode = StatusCodes.Status500InternalServerError;
            var traceId = httpContext.TraceIdentifier;

            var response = new ApiErrorResponse(
                Success: false,
                Message: "An unexpected error occurred.",
                Errors: null,
                TraceId: traceId);

            await httpContext.Response.WriteAsJsonAsync(response,cancellationToken);

            return true;
        }
    }
}
