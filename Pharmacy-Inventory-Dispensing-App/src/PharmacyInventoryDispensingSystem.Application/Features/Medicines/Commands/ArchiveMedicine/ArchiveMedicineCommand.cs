using MediatR;
using PharmacyInventoryDispensingSystem.Domain.Common.Results;
using System;

namespace PharmacyInventoryDispensingSystem.Application.Features.Medicines.Commands.ArchiveMedicine
{
    public sealed record ArchiveMedicineCommand(Guid MedicineId) : IRequest<Result<Deleted>>;
}
