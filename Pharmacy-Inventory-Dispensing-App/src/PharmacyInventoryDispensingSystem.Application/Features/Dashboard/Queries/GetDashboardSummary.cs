using MediatR;
using PharmacyInventoryDispensingSystem.Application.Features.Dashboard.Dtos;
using PharmacyInventoryDispensingSystem.Domain.Common.Results;
using System;
using System.Collections.Generic;
using System.Text;

namespace PharmacyInventoryDispensingSystem.Application.Features.Dashboard.Queries
{
    public sealed record GetDashboardSummaryQuery
    : IRequest<Result<DashboardSummaryDto>>;
}
