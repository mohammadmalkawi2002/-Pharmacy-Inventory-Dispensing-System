namespace PharmacyInventoryDispensingSystem.WebApi.Contracts.Requests.Authentication
{
    public record RegisterRequest(
        string email,
        string password,
        string FirstName,
        string LastName,
        string Role
        );


}
