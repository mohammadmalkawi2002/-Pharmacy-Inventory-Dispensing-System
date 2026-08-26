namespace PharmacyInventoryDispensingSystem.WebApi.Contracts.Requests.Authentication
{
    public record LoginRequest(
        string email,
        string password);
}
