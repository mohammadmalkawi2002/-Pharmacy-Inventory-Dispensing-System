using System.IO;

namespace PharmacyInventoryDispensingSystem.Application.Features.Medicines.Dtos
{
    public sealed record GetMedicineImageResponse(Stream Content, string ContentType);
}
