using MediatR;
using PharmacyInventoryDispensingSystem.Application.Common.Models;
using PharmacyInventoryDispensingSystem.Application.Features.Dispenses.Dtos;
using PharmacyInventoryDispensingSystem.Domain.Common.Results;
using System;
using System.Collections.Generic;
using System.Text;

namespace PharmacyInventoryDispensingSystem.Application.Features.Dispenses.Queries.GetDispenses
{
    public sealed record GetDispensesQuery(
        string? SearchTerm = null,
        DateOnly? FromDate = null,
        DateOnly? ToDate = null,
        int PageNumber = 1,
        int PageSize = 10)
      : IRequest<Result<PaginatedList<DispenseResponseDto>>>;
}
