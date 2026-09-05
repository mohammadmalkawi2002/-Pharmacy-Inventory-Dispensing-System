using MediatR;
using PharmacyInventoryDispensingSystem.Domain.Common.Results;
using System;
using System.Collections.Generic;
using System.Text;

namespace PharmacyInventoryDispensingSystem.Application.Features.Prescriptions.Commands.CancelPrescription
{
    public sealed record CancelPrescriptionCommand(Guid PrescriptionId)
    : IRequest<Result<Updated>>;
}
