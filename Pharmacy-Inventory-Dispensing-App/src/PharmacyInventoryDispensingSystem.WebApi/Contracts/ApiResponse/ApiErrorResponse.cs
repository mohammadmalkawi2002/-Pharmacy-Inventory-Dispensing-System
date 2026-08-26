namespace PharmacyInventoryDispensingSystem.WebApi.Contracts.ApiResponse
{
    public sealed record ApiErrorResponse(
     bool Success,
    string Message,
    object? Errors,
    string? TraceId);
    
    
}
