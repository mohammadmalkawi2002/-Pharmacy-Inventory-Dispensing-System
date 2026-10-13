using Serilog.Context;

namespace PharmacyInventoryDispensingSystem.WebApi.Middlewares
{
    public class RequestLogContextMiddleware(RequestDelegate next)
    {
        private readonly RequestDelegate _next=next;

        public async Task InvokeAsync(HttpContext httpContext)
        {
            using (LogContext.PushProperty("CorrelationId", httpContext.TraceIdentifier))
            {

                // the purpose is pushing the request correlation id into the log context
                // to be included in the structured log of a life time of http request
                await _next(httpContext);
            }
        }
    }
}
