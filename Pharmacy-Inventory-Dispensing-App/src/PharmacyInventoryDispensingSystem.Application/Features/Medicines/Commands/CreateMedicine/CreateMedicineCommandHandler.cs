using MediatR;
using Microsoft.Extensions.Logging;
using PharmacyInventoryDispensingSystem.Application.Common.Interfaces.Repositories;
using PharmacyInventoryDispensingSystem.Application.Features.Medicines.Dtos;
using PharmacyInventoryDispensingSystem.Application.Features.Medicines.Mappers;
using PharmacyInventoryDispensingSystem.Domain.Common.Results;
using PharmacyInventoryDispensingSystem.Domain.Entities.Medicines;
using System.Threading;
using System.Threading.Tasks;

namespace PharmacyInventoryDispensingSystem.Application.Features.Medicines.Commands.CreateMedicine
{
    public sealed class CreateMedicineCommandHandler(
        IMedicineRepository medicineRepository,
        IUnitOfWork unitOfWork,
        ILogger<CreateMedicineCommandHandler> logger)
        : IRequestHandler<CreateMedicineCommand, Result<MedicineDetailsResponseDto>>
    {
        public async Task<Result<MedicineDetailsResponseDto>> Handle(
            CreateMedicineCommand request,
            CancellationToken cancellationToken)
        {
            var code = request.Code.Trim();

            var codeExists = await medicineRepository.ExistsByCodeAsync(
                code,
                cancellationToken);

            if (codeExists)
            {
                logger.LogWarning("Medicine creation aborted. Code '{Code}' already exists.", code);
                return MedicineErrors.CodeConflict;
            }

            var medicine = new Medicine
            {
                Code = code,
                Name = request.Name.Trim(),
                Strength = request.Strength.Trim(),
                Form = request.Form,

                StockUnit = request.StockUnit,
                PackageUnit = request.PackageUnit,
                UnitsPerPackage = request.UnitsPerPackage,

                ReorderLevel = request.ReorderLevel,
                IsActive = true


            };

            await medicineRepository.AddAsync(medicine, cancellationToken);
            await unitOfWork.SaveChangesAsync(cancellationToken);

            logger.LogInformation("Medicine {MedicineId} was created successfully.", medicine.Id);

            return medicine.ToDetailsDto();
        }
    }
}
