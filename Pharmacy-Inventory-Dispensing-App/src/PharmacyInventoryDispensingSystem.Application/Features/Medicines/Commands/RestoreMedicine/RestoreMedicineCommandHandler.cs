using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using PharmacyInventoryDispensingSystem.Application.Common.Interfaces.Repositories;
using PharmacyInventoryDispensingSystem.Domain.Common.Results;
using PharmacyInventoryDispensingSystem.Domain.Entities.Medicines;
using System.Threading;
using System.Threading.Tasks;

namespace PharmacyInventoryDispensingSystem.Application.Features.Medicines.Commands.RestoreMedicine
{
    public sealed class RestoreMedicineCommandHandler(
      IGenericRepository<Medicine> medicineRepository,
        IUnitOfWork unitOfWork,
        ILogger<RestoreMedicineCommandHandler> logger)
        : IRequestHandler<RestoreMedicineCommand, Result<Updated>>
    {
        public async Task<Result<Updated>> Handle(
            RestoreMedicineCommand request,
            CancellationToken cancellationToken)
        {
            // MustBypass global query filter and track changes so Restore works:
            var medicine = await medicineRepository
                        .QueryIncludingDeleted(trackChanges:true)
                        .FirstOrDefaultAsync(m=>m.Id==request.MedicineId,cancellationToken);

            if (medicine is null)
            {
                logger.LogWarning("Medicine {MedicineId} was not found. Restore was rejected.", request.MedicineId);
                return MedicineErrors.NotFound(request.MedicineId);
            }

            if (!medicine.IsDeleted)
            {
                logger.LogWarning("Medicine {MedicineId} is not archived. Restore was rejected.", request.MedicineId);
                return MedicineErrors.NotArchived(request.MedicineId);
            }

            medicine.Restore();
            await unitOfWork.SaveChangesAsync(cancellationToken);

            logger.LogInformation("Medicine {MedicineId} was restored successfully.", request.MedicineId);

            return Result.Updated;
        }
    }
}
