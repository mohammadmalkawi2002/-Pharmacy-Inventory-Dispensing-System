using PharmacyInventoryDispensingSystem.Application.Features.Patients.Common;

namespace PharmacyInventoryDispensingSystem.WebApi.Contracts.Requests.Patient
{

    public sealed record GetPatientsRequest(
        string? SearchTerm = null,
        PatientDocumentType? DocumentType = null,
        string? SortBy = null,
        bool IsDescending = true,
        int PageNumber = 1,
        int PageSize = 10);
}
