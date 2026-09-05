namespace PharmacyInventoryDispensingSystem.WebApi.Contracts.Requests.User
{
    public sealed record CreateUserRequest(
        string FirstName,
        string LastName,
        string Email,
        string Password,
        string Role);
}
