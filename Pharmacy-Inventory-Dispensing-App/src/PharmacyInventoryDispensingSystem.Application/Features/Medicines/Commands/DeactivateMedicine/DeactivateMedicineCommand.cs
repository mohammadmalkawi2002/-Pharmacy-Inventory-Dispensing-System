using MediatR;
using PharmacyInventoryDispensingSystem.Domain.Common.Results;
using System;

namespace PharmacyInventoryDispensingSystem.Application.Features.Medicines.Commands.DeactivateMedicine
{
    public sealed record DeactivateMedicineCommand(Guid MedicineId) : IRequest<Result<Updated>>;
}
