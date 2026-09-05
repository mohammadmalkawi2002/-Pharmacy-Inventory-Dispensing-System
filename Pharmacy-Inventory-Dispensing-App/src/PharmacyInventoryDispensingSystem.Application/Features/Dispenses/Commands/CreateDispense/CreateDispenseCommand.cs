using MediatR;
using PharmacyInventoryDispensingSystem.Application.Features.Dispenses.Dtos;
using PharmacyInventoryDispensingSystem.Domain.Common.Results;
using System;
using System.Collections.Generic;
using System.Text;

namespace PharmacyInventoryDispensingSystem.Application.Features.Dispenses.Commands.CreateDispense
{
    public sealed record CreateDispenseCommand(
     Guid PrescriptionId,
     string DocumentId,
     IReadOnlyCollection<Guid> PrescriptionItemIds,
     string? Notes)
     : IRequest<Result<DispenseDetailsDto>>;
}
