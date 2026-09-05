using PharmacyInventoryDispensingSystem.Domain.Enums;

namespace PharmacyInventoryDispensingSystem.WebApi.Contracts.Requests.Prescriptions
{
    public sealed record GetPrescriptionsRequest(
     string? SearchTerm = null,
     PrescriptionStatus? Status = null,
     string SortBy = "CreatedAtUtc",
     bool IsDescending = true,
     int PageNumber = 1,
     int PageSize = 10);
}
