namespace PharmacyInventoryDispensingSystem.WebApi.Contracts.Requests.User
{
    public sealed record UpdateUserRequest(
        string FirstName,
        string LastName,
        string Email,
        string Role);
}
