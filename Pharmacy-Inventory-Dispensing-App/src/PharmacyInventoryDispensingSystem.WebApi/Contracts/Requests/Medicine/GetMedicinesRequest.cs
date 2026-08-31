using PharmacyInventoryDispensingSystem.Domain.Enums;

namespace PharmacyInventoryDispensingSystem.WebApi.Contracts.Requests.Medicine
{
    public sealed record GetMedicinesRequest(
        string? SearchTerm = null,
        MedicineForm? Form = null,
        StockUnit? StockUnit=null,
        bool? IsActive = null,
        string? SortBy = null,
        bool IsDescending = true,
        int PageNumber = 1,
        int PageSize = 10);
}
