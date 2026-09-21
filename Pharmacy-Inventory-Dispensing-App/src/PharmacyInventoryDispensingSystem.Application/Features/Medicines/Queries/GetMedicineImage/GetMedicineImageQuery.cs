using MediatR;
using PharmacyInventoryDispensingSystem.Application.Features.Medicines.Dtos;
using PharmacyInventoryDispensingSystem.Domain.Common.Results;
using System;

namespace PharmacyInventoryDispensingSystem.Application.Features.Medicines.Queries.GetMedicineImage
{
    public sealed record GetMedicineImageQuery(Guid MedicineId)
        : IRequest<Result<GetMedicineImageResponse>>;
}
