namespace PharmacyInventoryDispensingSystem.WebApi.Contracts.Requests.Prescriptions
{
    public sealed record UpdatePrescriptionRequest(
     DateOnly ValidFrom,
     DateOnly ValidTo,
     string? Notes,
     List<UpdatePrescriptionItemRequest> Items);

    public sealed record UpdatePrescriptionItemRequest(
        Guid MedicineId,
        int QuantityPrescribed,
        int MaxFillCount,
        string? DosageInstructions);
}
