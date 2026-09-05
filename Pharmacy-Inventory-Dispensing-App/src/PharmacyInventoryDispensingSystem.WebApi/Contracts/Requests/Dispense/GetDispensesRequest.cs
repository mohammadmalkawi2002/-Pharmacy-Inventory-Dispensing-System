namespace PharmacyInventoryDispensingSystem.WebApi.Contracts.Requests.Dispense
{
    public sealed record GetDispensesRequest(
     string? SearchTerm = null,
     DateOnly? FromDate = null,
     DateOnly? ToDate = null,
     int PageNumber = 1,
     int PageSize = 10);
}
