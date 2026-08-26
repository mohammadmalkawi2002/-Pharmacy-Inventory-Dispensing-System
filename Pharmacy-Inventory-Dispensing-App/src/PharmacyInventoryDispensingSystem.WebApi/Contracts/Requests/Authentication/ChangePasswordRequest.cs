namespace PharmacyInventoryDispensingSystem.WebApi.Contracts.Requests.Authentication
{
    public record ChangePasswordRequest(string currentPassword, string newPassword);
    
    
}
