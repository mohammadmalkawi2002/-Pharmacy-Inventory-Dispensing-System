using MediatR;
using PharmacyInventoryDispensingSystem.Application.Common.Models;
using PharmacyInventoryDispensingSystem.Application.Features.Medicines.Dtos;
using PharmacyInventoryDispensingSystem.Domain.Common.Results;

namespace PharmacyInventoryDispensingSystem.Application.Features.Medicines.Queries.GetArchivedMedicines
{
    public sealed record GetArchivedMedicinesQuery(
        string? SearchTerm = null,
        int PageNumber = 1,
        int PageSize = 10)
        : IRequest<Result<PaginatedList<MedicineResponseDto>>>;
}
