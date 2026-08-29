namespace PharmacyInventoryDispensingSystem.WebApi.Contracts.Requests.Patient
{
    public sealed record CreatePatientRequest(
    string DocumentId,
    string FullName,
    DateTime DateOfBirth,
    string PhoneNumber);
}
