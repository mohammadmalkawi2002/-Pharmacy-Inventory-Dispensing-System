using MediatR;
using PharmacyInventoryDispensingSystem.Domain.Common.Results;
using System;

namespace PharmacyInventoryDispensingSystem.Application.Features.Medicines.Commands.ActivateMedicine
{
    public sealed record ActivateMedicineCommand(Guid MedicineId) : IRequest<Result<Updated>>;
}
