using MediatR;
using PharmacyInventoryDispensingSystem.Application.Features.Prescriptions.Dtos;
using PharmacyInventoryDispensingSystem.Domain.Common.Results;
using System;
using System.Collections.Generic;
using System.Text;

namespace PharmacyInventoryDispensingSystem.Application.Features.Prescriptions.Commands.CreatePrescription
{
    public sealed record CreatePrescriptionCommand(
        Guid PatientId,
        DateOnly ValidFrom,
        DateOnly ValidTo,
        string? Notes,
        List<CreatePrescriptionItemCommand> Items

    ) : IRequest<Result<CreatePrescriptionResponse>>;
}
