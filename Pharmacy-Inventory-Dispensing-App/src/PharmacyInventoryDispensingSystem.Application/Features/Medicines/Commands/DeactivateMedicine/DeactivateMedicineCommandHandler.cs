using MediatR;
using Microsoft.Extensions.Logging;
using PharmacyInventoryDispensingSystem.Application.Common.Interfaces.Repositories;
using PharmacyInventoryDispensingSystem.Domain.Common.Results;
using PharmacyInventoryDispensingSystem.Domain.Entities.Medicines;
using System.Threading;
using System.Threading.Tasks;

namespace PharmacyInventoryDispensingSystem.Application.Features.Medicines.Commands.DeactivateMedicine
{
    public sealed class DeactivateMedicineCommandHandler(
        IMedicineRepository medicineRepository,
        IUnitOfWork unitOfWork,
        ILogger<DeactivateMedicineCommandHandler> logger)
        : IRequestHandler<DeactivateMedicineCommand, Result<Updated>>
    {
        public async Task<Result<Updated>> Handle(
            DeactivateMedicineCommand request,
            CancellationToken cancellationToken)
        {
            var medicine = await medicineRepository.GetByIdAsync(
                request.MedicineId,
                trackChanges: true,
                cancellationToken: cancellationToken);

            if (medicine is null)
            {
                logger.LogWarning("Medicine {MedicineId} was not found. Deactivation rejected.", request.MedicineId);
                return MedicineErrors.NotFound(request.MedicineId);
            }

            if (!medicine.IsActive)
            {
                logger.LogWarning("Medicine {MedicineId} is already inactive.", request.MedicineId);
                return MedicineErrors.AlreadyInactive;
            }

            medicine.IsActive = false;
            await unitOfWork.SaveChangesAsync(cancellationToken);

            logger.LogInformation("Medicine {MedicineId} was deactivated successfully.", request.MedicineId);

            return Result.Updated;
        }
    }
}
