namespace PharmacyInventoryDispensingSystem.WebApi.Contracts.Requests.Authentication
{
    public record ResetPasswordRequest(string Email,
        string Token,
        string NewPassword);
    
}
