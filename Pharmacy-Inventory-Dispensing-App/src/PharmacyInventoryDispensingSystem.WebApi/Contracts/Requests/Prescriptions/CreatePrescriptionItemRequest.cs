namespace PharmacyInventoryDispensingSystem.WebApi.Contracts.Requests.Prescriptions
{
    public sealed record CreatePrescriptionItemRequest(
     Guid MedicineId,
     int QuantityPrescribed,
     int MaxFillCount,
     string? DosageInstructions);
}
