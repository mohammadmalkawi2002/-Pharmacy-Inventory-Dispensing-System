using MediatR;
using PharmacyInventoryDispensingSystem.Domain.Common.Results;
using System;
using System.Collections.Generic;
using System.Text;

namespace PharmacyInventoryDispensingSystem.Application.Features.Prescriptions.Commands.UpdatePrescription
{
    public sealed record UpdatePrescriptionCommand(
        Guid PrescriptionId,
        DateOnly ValidFrom,
        DateOnly ValidTo,
        string? Notes,
        List<UpdatePrescriptionItemCommand> Items)
    : IRequest<Result<Updated>>;
    
}
