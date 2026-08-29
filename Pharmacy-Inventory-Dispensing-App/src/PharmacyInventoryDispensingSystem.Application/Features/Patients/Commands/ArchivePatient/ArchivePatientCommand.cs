using MediatR;
using PharmacyInventoryDispensingSystem.Domain.Common.Results;
using System;
using System.Collections.Generic;
using System.Text;

namespace PharmacyInventoryDispensingSystem.Application.Features.Patients.Commands.ArchivePatient
{
    public sealed record ArchivePatientCommand(Guid PatientId) : IRequest<Result<Deleted>>;
    
}
