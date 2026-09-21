using MediatR;
using PharmacyInventoryDispensingSystem.Application.Common.Files;
using PharmacyInventoryDispensingSystem.Domain.Common.Results;
using PharmacyInventoryDispensingSystem.Domain.Enums;
using System;

namespace PharmacyInventoryDispensingSystem.Application.Features.Medicines.Commands.UpdateMedicine
{
    public sealed record UpdateMedicineCommand(
        Guid MedicineId,
        string Code,
        string Name,
        string Strength,
        MedicineForm Form,
        StockUnit StockUnit,
        PackageUnit PackageUnit,
        int UnitsPerPackage,
        int ReorderLevel,
       UploadedFile? Image)
        : IRequest<Result<Updated>>;
}
