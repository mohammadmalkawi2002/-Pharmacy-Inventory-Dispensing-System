namespace PharmacyInventoryDispensingSystem.WebApi.Contracts.Requests.Prescriptions
{
    public sealed record LookupPrescriptionRequest(
    string PrescriptionNumber,
    string DocumentId);
}
