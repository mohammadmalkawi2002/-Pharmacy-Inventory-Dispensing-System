namespace PharmacyInventoryDispensingSystem.WebApi.Contracts.Requests.User
{
    public sealed record GetUsersRequest(
        string? SearchTerm = null,
        string? Role = null,
        int PageNumber = 1,
        int PageSize = 10);
}
