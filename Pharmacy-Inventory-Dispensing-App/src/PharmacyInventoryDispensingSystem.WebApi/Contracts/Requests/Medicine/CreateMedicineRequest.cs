using PharmacyInventoryDispensingSystem.Domain.Enums;

namespace PharmacyInventoryDispensingSystem.WebApi.Contracts.Requests.Medicine
{
    public sealed record CreateMedicineRequest(
        string Code,
        string Name,
        string Strength,
        MedicineForm Form,
        StockUnit StockUnit,
        PackageUnit PackageUnit,
        int UnitsPerPackage,
        int ReorderLevel);
}

