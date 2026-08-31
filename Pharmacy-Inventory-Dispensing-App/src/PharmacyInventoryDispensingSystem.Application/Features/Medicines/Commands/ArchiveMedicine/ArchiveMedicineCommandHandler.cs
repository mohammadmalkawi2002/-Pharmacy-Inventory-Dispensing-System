using MediatR;
using Microsoft.Extensions.Logging;
using PharmacyInventoryDispensingSystem.Application.Common.Interfaces.Repositories;
using PharmacyInventoryDispensingSystem.Domain.Common.Results;
using PharmacyInventoryDispensingSystem.Domain.Entities.Medicines;
using System.Threading;
using System.Threading.Tasks;

namespace PharmacyInventoryDispensingSystem.Application.Features.Medicines.Commands.ArchiveMedicine
{
    public sealed class ArchiveMedicineCommandHandler(
        IMedicineRepository medicineRepository,
        IUnitOfWork unitOfWork,
        ILogger<ArchiveMedicineCommandHandler> logger)
        : IRequestHandler<ArchiveMedicineCommand, Result<Deleted>>
    {
        public async Task<Result<Deleted>> Handle(
            ArchiveMedicineCommand request,
            CancellationToken cancellationToken)
        {
            var medicine = await medicineRepository.GetByIdIncludingArchivedAsync(
                request.MedicineId,
                cancellationToken);

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

            medicine.Delete();
            await unitOfWork.SaveChangesAsync(cancellationToken);

            logger.LogInformation("Medicine {MedicineId} was archived successfully.", request.MedicineId);

            return Result.Deleted;
        }
    }
}
