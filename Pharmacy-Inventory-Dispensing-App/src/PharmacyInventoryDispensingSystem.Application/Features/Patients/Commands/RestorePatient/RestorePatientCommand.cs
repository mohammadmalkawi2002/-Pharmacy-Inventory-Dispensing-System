using MediatR;
using PharmacyInventoryDispensingSystem.Domain.Common.Results;
using System;
using System.Collections.Generic;
using System.Text;

namespace PharmacyInventoryDispensingSystem.Application.Features.Patients.Commands.RestorePatient
{
    public sealed record RestorePatientCommand(Guid PatientId)
    : IRequest<Result<Updated>>;
}
