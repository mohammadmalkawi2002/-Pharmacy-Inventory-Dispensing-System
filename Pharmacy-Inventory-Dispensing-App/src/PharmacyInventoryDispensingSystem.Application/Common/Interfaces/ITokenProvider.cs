namespace PharmacyInventoryDispensingSystem.Application.Common.Interfaces;

public interface ITokenProvider
{
    string GenerateAccessToken(string userId, string email, IEnumerable<string> roles);
}
