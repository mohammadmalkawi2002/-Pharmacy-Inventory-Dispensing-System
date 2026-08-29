namespace PharmacyInventoryDispensingSystem.WebApi.Contracts.Requests.Authentication
{
    public record ChangePasswordRequest(string CurrentPassword, string NewPassword);
    
    
}
