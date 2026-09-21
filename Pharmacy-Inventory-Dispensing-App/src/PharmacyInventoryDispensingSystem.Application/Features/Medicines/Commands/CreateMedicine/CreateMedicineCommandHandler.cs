using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using PharmacyInventoryDispensingSystem.Application.Common.Interfaces;
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
        IFileImageService fileImageService,
        IUnitOfWork unitOfWork,
        ILogger<CreateMedicineCommandHandler> logger)
        : IRequestHandler<CreateMedicineCommand, Result<MedicineDetailsResponseDto>>
    {
        public async Task<Result<MedicineDetailsResponseDto>> Handle(
            CreateMedicineCommand request,
            CancellationToken cancellationToken)
        {
            var code = request.Code.Trim();

            var codeExists = await medicineRepository.ExistsByCodeAsync(code, cancellationToken);

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

            if (request.Image is not null)
            {
                var uploadResult = await fileImageService.UploadAsync(request.Image, cancellationToken);
                if (uploadResult.IsError)
                {
                    return uploadResult.Errors;
                }

                medicine.ImageId = uploadResult.Value.Id;
            }

            medicineRepository.Add(medicine);
            await unitOfWork.SaveChangesAsync(cancellationToken);

            logger.LogInformation("Medicine {MedicineId} was created successfully.", medicine.Id);

            return medicine.ToDetailsDto();
        }
    }
}
