namespace PharmacyInventoryDispensingSystem.WebApi.Contracts.Requests.Dispense
{
    public sealed record CreateDispenseRequest(
    Guid PrescriptionId,
    string DocumentId,
    IReadOnlyCollection<Guid> PrescriptionItemIds,
    string? Notes);
}
