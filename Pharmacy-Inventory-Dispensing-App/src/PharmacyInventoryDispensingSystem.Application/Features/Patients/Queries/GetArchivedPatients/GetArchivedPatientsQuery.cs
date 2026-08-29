using MediatR;
using PharmacyInventoryDispensingSystem.Application.Common.Models;
using PharmacyInventoryDispensingSystem.Application.Features.Patients.Common;
using PharmacyInventoryDispensingSystem.Application.Features.Patients.Dtos;
using PharmacyInventoryDispensingSystem.Domain.Common.Results;
using System;
using System.Collections.Generic;
using System.Text;

namespace PharmacyInventoryDispensingSystem.Application.Features.Patients.Queries.GetArchivedPatients
{
    public sealed record GetArchivedPatientsQuery(
        string? SearchTerm = null,
        PatientDocumentType? DocumentType = null,
        string? SortBy = null,
        bool IsDescending = true,
        int PageNumber = 1,
        int PageSize = 10) : IRequest<Result<PaginatedList<PatientResponseDto>>>;
    
}
