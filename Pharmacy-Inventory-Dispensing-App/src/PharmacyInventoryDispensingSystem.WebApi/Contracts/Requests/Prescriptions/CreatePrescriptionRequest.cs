namespace PharmacyInventoryDispensingSystem.WebApi.Contracts.Requests.Prescriptions
{
    public sealed record CreatePrescriptionRequest(
      Guid PatientId,
      DateOnly ValidFrom,
      DateOnly ValidTo,
      string? Notes,
      List<CreatePrescriptionItemRequest> Items);
}
