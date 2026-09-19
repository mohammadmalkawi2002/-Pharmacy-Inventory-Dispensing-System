using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using PharmacyInventoryDispensingSystem.Application.Common.Interfaces.Repositories;
using PharmacyInventoryDispensingSystem.Domain.Common.Results;
using PharmacyInventoryDispensingSystem.Domain.Entities.Medicines;
using System.Threading;
using System.Threading.Tasks;

namespace PharmacyInventoryDispensingSystem.Application.Features.Medicines.Commands.ArchiveMedicine
{
    public sealed class ArchiveMedicineCommandHandler(
        IGenericRepository<Medicine> medicineRepository,
        IUnitOfWork unitOfWork,
        ILogger<ArchiveMedicineCommandHandler> logger)
        : IRequestHandler<ArchiveMedicineCommand, Result<Deleted>>
    {
        public async Task<Result<Deleted>> Handle(
            ArchiveMedicineCommand request,
            CancellationToken cancellationToken)
        { 
            var medicine = await medicineRepository
                            .QueryIncludingDeleted(trackChanges: true)
                           .FirstOrDefaultAsync(m => m.Id == request.MedicineId, cancellationToken);

            if (medicine is null)
            {
                logger.LogWarning("Medicine {MedicineId} was not found. Archive rejected.", request.MedicineId);
                return MedicineErrors.NotFound(request.MedicineId);
            }

            if (medicine.IsDeleted)
            {
                logger.LogWarning("Medicine {MedicineId} is already archived.", request.MedicineId);
                return MedicineErrors.AlreadyArchived(request.MedicineId);
            }

            //Apply softDelete:
            medicineRepository.Delete(medicine);
            await unitOfWork.SaveChangesAsync(cancellationToken);

            logger.LogInformation("Medicine {MedicineId} was archived successfully.", request.MedicineId);

            return Result.Deleted;
        }
    }
}
