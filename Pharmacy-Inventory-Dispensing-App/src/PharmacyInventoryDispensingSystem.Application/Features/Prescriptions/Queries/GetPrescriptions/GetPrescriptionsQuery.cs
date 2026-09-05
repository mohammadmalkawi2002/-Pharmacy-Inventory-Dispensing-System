using MediatR;
using PharmacyInventoryDispensingSystem.Application.Common.Models;
using PharmacyInventoryDispensingSystem.Application.Features.Patients.Common;
using PharmacyInventoryDispensingSystem.Application.Features.Patients.Dtos;
using PharmacyInventoryDispensingSystem.Application.Features.Prescriptions.Dtos;
using PharmacyInventoryDispensingSystem.Domain.Common.Results;
using PharmacyInventoryDispensingSystem.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace PharmacyInventoryDispensingSystem.Application.Features.Prescriptions.Queries.GetPrescriptions
{
    public sealed record GetPrescriptionsQuery(
        string? SearchTerm = null,
        PrescriptionStatus? Status = null,
        string? SortBy = null,
        bool IsDescending = true,
        int PageNumber = 1,
        int PageSize = 10)
    : IRequest<Result<PaginatedList<PrescriptionSummaryDto>>>;
}
