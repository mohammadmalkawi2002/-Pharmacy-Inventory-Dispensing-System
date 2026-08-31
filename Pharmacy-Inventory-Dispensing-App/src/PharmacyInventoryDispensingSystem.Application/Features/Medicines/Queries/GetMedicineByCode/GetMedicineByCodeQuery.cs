using MediatR;
using PharmacyInventoryDispensingSystem.Application.Features.Medicines.Dtos;
using PharmacyInventoryDispensingSystem.Domain.Common.Results;

namespace PharmacyInventoryDispensingSystem.Application.Features.Medicines.Queries.GetMedicineByCode
{
    public sealed record GetMedicineByCodeQuery(string Code)
        : IRequest<Result<MedicineDetailsResponseDto>>;
}
