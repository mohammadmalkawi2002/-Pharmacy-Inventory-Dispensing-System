using PharmacyInventoryDispensingSystem.Domain.Entities.Prescriptions;
using PharmacyInventoryDispensingSystem.Domain.Enums;

namespace PharmacyInventoryDispensingSystem.Application.Common.Interfaces.Repositories;

public interface IPrescriptionRepository : IGenericRepository<Prescription>
{
    Task<string> GenerateNextPrescriptionNumberAsync(
        CancellationToken cancellationToken = default);

   
    void RemoveItem(PrescriptionItem item);

    Task<Prescription?> GetForDispensingAsync(
        Guid prescriptionId,
        string documentId,
        CancellationToken cancellationToken = default);

   
    Task<Prescription?> GetByIdWithItemsAsync(
        Guid prescriptionId,
        CancellationToken cancellationToken = default);

   
}
