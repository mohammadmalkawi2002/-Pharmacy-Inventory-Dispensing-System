namespace PharmacyInventoryDispensingSystem.WebApi.Contracts.Requests.Patient
{
    public sealed record UpdatePatientRequest(
     string DocumentId,
     string FullName,
     DateTime DateOfBirth,
     string PhoneNumber);
}
