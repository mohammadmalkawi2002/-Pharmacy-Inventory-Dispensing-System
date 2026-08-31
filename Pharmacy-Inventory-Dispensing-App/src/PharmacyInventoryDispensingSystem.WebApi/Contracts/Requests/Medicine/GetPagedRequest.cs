namespace PharmacyInventoryDispensingSystem.WebApi.Contracts.Requests.Medicine
{
    public sealed record GetPagedRequest(
        int PageNumber = 1,
        int PageSize = 10);
}
