using MediatR;
using PharmacyInventoryDispensingSystem.Application.Common.Models;
using PharmacyInventoryDispensingSystem.Application.Features.Medicines.Dtos;
using PharmacyInventoryDispensingSystem.Domain.Common.Results;
using PharmacyInventoryDispensingSystem.Domain.Enums;

namespace PharmacyInventoryDispensingSystem.Application.Features.Medicines.Queries.GetLowStockMedicines
{
    public sealed record GetLowStockMedicinesQuery(
       string? SearchTerm = null,
        MedicineForm? Form = null,
        StockUnit? StockUnit = null,
        bool? IsActive = null,
        string? SortBy = null,
        bool IsDescending = true,
        int PageNumber = 1,
        int PageSize = 10)
        : IRequest<Result<PaginatedList<MedicineResponseDto>>>;
}
