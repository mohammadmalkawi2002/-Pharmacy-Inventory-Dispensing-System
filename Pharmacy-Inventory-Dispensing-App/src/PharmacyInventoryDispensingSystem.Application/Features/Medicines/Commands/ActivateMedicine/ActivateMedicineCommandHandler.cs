using MediatR;
using Microsoft.Extensions.Logging;
using PharmacyInventoryDispensingSystem.Application.Common.Interfaces.Repositories;
using PharmacyInventoryDispensingSystem.Domain.Common.Results;
using PharmacyInventoryDispensingSystem.Domain.Entities.Medicines;
using System.Threading;
using System.Threading.Tasks;

namespace PharmacyInventoryDispensingSystem.Application.Features.Medicines.Commands.ActivateMedicine
{
    public sealed class ActivateMedicineCommandHandler(
        IMedicineRepository medicineRepository,
        IUnitOfWork unitOfWork,
        ILogger<ActivateMedicineCommandHandler> logger)
        : IRequestHandler<ActivateMedicineCommand, Result<Updated>>
    {
        public async Task<Result<Updated>> Handle(
            ActivateMedicineCommand request,
            CancellationToken cancellationToken)
        {
            var medicine = await medicineRepository.GetByIdAsync(
                request.MedicineId,
                trackChanges: true,
                cancellationToken: cancellationToken);

            if (medicine is null)
            {
                logger.LogWarning("Medicine {MedicineId} was not found. Activation rejected.", request.MedicineId);
                return MedicineErrors.NotFound(request.MedicineId);
            }

            if (medicine.IsActive)
            {
                logger.LogWarning("Medicine {MedicineId} is already active.", request.MedicineId);
                return MedicineErrors.AlreadyActive;
            }

            medicine.IsActive = true;
            await unitOfWork.SaveChangesAsync(cancellationToken);

            logger.LogInformation("Medicine {MedicineId} was activated successfully.", request.MedicineId);

            return Result.Updated;
        }
    }
}
