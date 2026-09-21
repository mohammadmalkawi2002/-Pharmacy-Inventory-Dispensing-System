using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using PharmacyInventoryDispensingSystem.Application.Common.Errors;
using PharmacyInventoryDispensingSystem.Application.Common.Interfaces;
using PharmacyInventoryDispensingSystem.Application.Common.Interfaces.Repositories;
using PharmacyInventoryDispensingSystem.Domain.Common.Results;
using PharmacyInventoryDispensingSystem.Domain.Entities.FileSystem;
using PharmacyInventoryDispensingSystem.Domain.Entities.Medicines;
using PharmacyInventoryDispensingSystem.Domain.Entities.Patients;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace PharmacyInventoryDispensingSystem.Application.Features.Medicines.Commands.UpdateMedicine
{
    public sealed class UpdateMedicineCommandHandler(
        IMedicineRepository medicineRepository,
        IFileImageService fileImageService,
        IGenericRepository<FileImage> fileImageRepository,
        IUnitOfWork unitOfWork,
        ILogger<UpdateMedicineCommandHandler> logger)
        : IRequestHandler<UpdateMedicineCommand, Result<Updated>>
    {
        public async Task<Result<Updated>> Handle(
            UpdateMedicineCommand request,
            CancellationToken cancellationToken)
        {
            var medicine = await medicineRepository.GetByIdAsync(
                request.MedicineId,
                cancellationToken: cancellationToken);

            if (medicine is null)
            {
                logger.LogWarning("Medicine {MedicineId} not found. Update was rejected.", request.MedicineId);
                return MedicineErrors.NotFound(request.MedicineId);
            }

            //→ Check Code Uniqueness: if code changed only check the code in db (ite mean hit the db only if code changed)
            var code = request.Code.Trim();
            bool codeChanged = !string.Equals(
                medicine.Code,
                code,
                StringComparison.OrdinalIgnoreCase);


            if (codeChanged)
            {
                var codeExists = await medicineRepository.ExistsByCodeAsync(code, cancellationToken);

                if (codeExists)
                {
                    logger.LogWarning(
                        "Medicine update was rejected for {MedicineId} because a medicine with code '{Code}' already exists.",
                        medicine.Id,
                        code);
                    return MedicineErrors.CodeConflict;
                }
            }


            //→ Validate Stock Configuration Change
            var stockConfigurationResult = await ValidateStockConfigurationChangeAsync(medicine,
                                                                                 request,
                                                                                 cancellationToken);


            if (stockConfigurationResult.IsError) 
            {
                logger.LogWarning(
                    "Stock configuration change was rejected for medicine {MedicineId}.",
                    medicine.Id);

                return stockConfigurationResult.Errors;

            }

            //→ Keep old image for cleanup.
            FileImage? obsoleteFileImage = null;

            if (request.Image is not null)
            {
                var uploadResult = await fileImageService.UploadAsync(request.Image, cancellationToken);
                if (uploadResult.IsError)
                {
                    return uploadResult.Errors;
                }

                //Replace the old img with new 
                obsoleteFileImage = await ReplaceMedicineImageAsync(medicine, uploadResult.Value.Id, cancellationToken);
            }

            ApplyMedicineChanges(medicine, request, code);
            medicineRepository.Update(medicine);

            // SaveChanges #1: Commit Medicine update (points to new image if any)
            await unitOfWork.SaveChangesAsync(cancellationToken);

            // SaveChanges #2: Only if replacing an old image
            if (obsoleteFileImage is not null)
            {
                fileImageRepository.HardDelete(obsoleteFileImage);
                await unitOfWork.SaveChangesAsync(cancellationToken);
            }

            // Clean up old physical file after DB success.
            CleanupObsoletePhysicalImage(obsoleteFileImage);

            logger.LogInformation("Medicine {MedicineId} was updated successfully.", medicine.Id);

            return Result.Updated;
        }

        private async Task<FileImage?> ReplaceMedicineImageAsync(Medicine medicine, Guid newImageId, CancellationToken cancellationToken)
        {
            FileImage? obsoleteImage = null;
            // Get old image before replacing it.
            if (medicine.ImageId.HasValue)
            {
                obsoleteImage = await fileImageRepository.GetByIdAsync(medicine.ImageId.Value, cancellationToken);
            }

            medicine.ImageId = newImageId;
            return obsoleteImage;
        }

        private void CleanupObsoletePhysicalImage(FileImage? obsoleteFileImage)
        {
            if (obsoleteFileImage is null)
                return;

            try
            {
                fileImageService.DeletePhysicalFile(obsoleteFileImage.StoredFileName);
            }
            catch (Exception ex)
            {
                logger.LogWarning(
                    ex,
                    "Failed to clean up old physical image file {StoredFileName}.",
                    obsoleteFileImage.StoredFileName);
            }
        }

        private static void ApplyMedicineChanges(Medicine medicine, UpdateMedicineCommand request, string code)
        {
            medicine.Code = code;
            medicine.Name = request.Name.Trim();
            medicine.Strength = request.Strength.Trim();
            medicine.Form = request.Form;
            medicine.StockUnit = request.StockUnit;
            medicine.PackageUnit = request.PackageUnit;
            medicine.UnitsPerPackage = request.UnitsPerPackage;
            medicine.ReorderLevel = request.ReorderLevel;
        }


        private async Task<Result<Success>> ValidateStockConfigurationChangeAsync(
            Medicine medicine,
            UpdateMedicineCommand request,
            CancellationToken cancellationToken)
        {
            var stockConfigurationChanged =
          medicine.StockUnit != request.StockUnit ||
        medicine.PackageUnit != request.PackageUnit ||
        medicine.UnitsPerPackage != request.UnitsPerPackage;



            if (!stockConfigurationChanged)
                return Result.Success;


            if (medicine.QuantityInStock > 0)
                return MedicineErrors.StockConfigurationCannotBeChanged;

            var isUsedInPrescription = await medicineRepository.Query()
                        .AnyAsync(m => m.Id == medicine.Id && m.PrescriptionItems.Any(),
                            cancellationToken);



            if (isUsedInPrescription)
                return MedicineErrors.StockConfigurationCannotBeChanged;



            return Result.Success;
           
        }
    }
}
