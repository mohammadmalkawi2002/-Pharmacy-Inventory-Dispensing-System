using MediatR;
using PharmacyInventoryDispensingSystem.Domain.Common.Results;
using System;

namespace PharmacyInventoryDispensingSystem.Application.Features.Medicines.Commands.RestoreMedicine
{
    public sealed record RestoreMedicineCommand(Guid MedicineId) : IRequest<Result<Updated>>;
}
