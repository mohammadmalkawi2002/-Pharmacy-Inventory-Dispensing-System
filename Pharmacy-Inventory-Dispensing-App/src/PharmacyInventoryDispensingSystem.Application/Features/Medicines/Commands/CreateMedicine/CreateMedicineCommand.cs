using MediatR;
using PharmacyInventoryDispensingSystem.Application.Features.Medicines.Dtos;
using PharmacyInventoryDispensingSystem.Domain.Common.Results;
using PharmacyInventoryDispensingSystem.Domain.Enums;
using System;

namespace PharmacyInventoryDispensingSystem.Application.Features.Medicines.Commands.CreateMedicine
{
    public sealed record CreateMedicineCommand(
        string Code,
        string Name,
        string Strength,
        MedicineForm Form,
        StockUnit StockUnit,
        PackageUnit PackageUnit,
        int UnitsPerPackage,
        int ReorderLevel)
        : IRequest<Result<MedicineDetailsResponseDto>>;
}
