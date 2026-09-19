using Microsoft.EntityFrameworkCore;
using PharmacyInventoryDispensingSystem.Application.Common.Interfaces.Repositories;
using PharmacyInventoryDispensingSystem.Domain.Entities.Prescriptions;
using PharmacyInventoryDispensingSystem.Domain.Enums;
using PharmacyInventoryDispensingSystem.Infrastructure.Persistence.Context;

namespace PharmacyInventoryDispensingSystem.Infrastructure.Persistence.Repositories;

public class PrescriptionRepository(AppDbContext context)
    : GenericRepository<Prescription>(context), IPrescriptionRepository
{
    public async Task<string> GenerateNextPrescriptionNumberAsync(CancellationToken cancellationToken = default)
    {
        int sequenceValue = await DbContext.Database
            .SqlQuery<int>($"SELECT NEXT VALUE FOR PrescriptionNumberSequence AS Value")
            .SingleAsync(cancellationToken);

        return $"RX-{sequenceValue:D6}";
    }

    public async Task<Prescription?> GetForDispensingAsync(
        Guid prescriptionId,
        string documentId,
        CancellationToken cancellationToken = default)
    {
        // Tracked — medicines are loaded so stock can be mutated and persisted.
        return await DbContext.Prescriptions
            .Include(prescription => prescription.Patient)
            .Include(prescription => prescription.Items)
                .ThenInclude(item => item.Medicine)
            .SingleOrDefaultAsync(
                prescription =>
                    prescription.Id == prescriptionId &&
                    prescription.Patient.DocumentId == documentId,
                cancellationToken);
    }

  

    /// <summary>
    /// Retrieves a tracked prescription with its Items eagerly loaded.
    /// Used by update workflows that add, modify, or remove items.
    /// </summary>
    /// <remarks>
    /// <see cref="BaseRepository{TEntity}.GetByIdAsync"/> which returns a simple,
    /// no-tracking entity without navigation properties.
    /// </remarks>
    public async Task<Prescription?> GetByIdWithItemsAsync(
        Guid prescriptionId,
        CancellationToken cancellationToken = default)
    {
        return await DbContext.Prescriptions
         .Include(prescription => prescription.Items)
         .FirstOrDefaultAsync(
             prescription => prescription.Id == prescriptionId,
              cancellationToken);
    }
   
    public void RemoveItem(PrescriptionItem item)
    {
        DbContext.PrescriptionItems.Remove(item);
    }

   
}
