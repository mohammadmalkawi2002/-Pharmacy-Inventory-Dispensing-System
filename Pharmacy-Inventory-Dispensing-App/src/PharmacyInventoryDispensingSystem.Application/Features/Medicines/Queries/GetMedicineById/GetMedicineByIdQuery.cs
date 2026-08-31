using MediatR;
using PharmacyInventoryDispensingSystem.Application.Features.Medicines.Dtos;
using PharmacyInventoryDispensingSystem.Domain.Common.Results;
using System;

namespace PharmacyInventoryDispensingSystem.Application.Features.Medicines.Queries.GetMedicineById
{
    public sealed record GetMedicineByIdQuery(Guid MedicineId)
        : IRequest<Result<MedicineDetailsResponseDto>>;
}
