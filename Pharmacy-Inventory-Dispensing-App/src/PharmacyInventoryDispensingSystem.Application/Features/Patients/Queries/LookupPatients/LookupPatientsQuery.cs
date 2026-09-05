using MediatR;
using PharmacyInventoryDispensingSystem.Application.Features.Patients.Dtos;
using PharmacyInventoryDispensingSystem.Domain.Common.Results;
using System;
using System.Collections.Generic;
using System.Text;

namespace PharmacyInventoryDispensingSystem.Application.Features.Patients.Queries.LookupPatients
{
    public sealed record LookupPatientsQuery(
    string SearchTerm)
    : IRequest<Result<List<PatientLookupDto>>>;
}
