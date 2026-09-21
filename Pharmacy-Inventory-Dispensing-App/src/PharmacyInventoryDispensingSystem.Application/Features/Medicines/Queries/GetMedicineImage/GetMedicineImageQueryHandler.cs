using MediatR;
using Microsoft.Extensions.Logging;
using PharmacyInventoryDispensingSystem.Application.Common.Interfaces;
using PharmacyInventoryDispensingSystem.Application.Common.Interfaces.Repositories;
using PharmacyInventoryDispensingSystem.Application.Features.Medicines.Dtos;
using PharmacyInventoryDispensingSystem.Domain.Common.Results;
using PharmacyInventoryDispensingSystem.Domain.Entities.FileSystem;
using PharmacyInventoryDispensingSystem.Domain.Entities.Medicines;
using System.Threading;
using System.Threading.Tasks;

namespace PharmacyInventoryDispensingSystem.Application.Features.Medicines.Queries.GetMedicineImage
{
    public sealed class GetMedicineImageQueryHandler(
        IGenericRepository<Medicine> medicineRepository,
        IGenericRepository<FileImage> fileImageRepository,
        IFileImageService fileImageService,
        ILogger<GetMedicineImageQueryHandler> logger)
        : IRequestHandler<GetMedicineImageQuery, Result<GetMedicineImageResponse>>
    {
        public async Task<Result<GetMedicineImageResponse>> Handle(
            GetMedicineImageQuery request,
            CancellationToken cancellationToken)
        {
            var medicine = await medicineRepository.GetByIdAsync(
                request.MedicineId,
                cancellationToken);

            if (medicine is null)
            {
                logger.LogWarning("Medicine with ID {MedicineId} was not found.", request.MedicineId);
                return MedicineErrors.NotFound(request.MedicineId);
            }

            // The Medicine stores only the FileImage reference, not the physical file.:
            if (medicine.ImageId is null)
            {
                logger.LogWarning("Medicine with ID {MedicineId} does not have an image.", request.MedicineId);
                return Error.NotFound(
                    "MedicineImage.NotFound",
                    $"Medicine with ID '{request.MedicineId}' does not have an image.");
            }

            // Load the image metadata to resolve the stored file name and content type.
            var fileImage = await fileImageRepository.GetByIdAsync(
                medicine.ImageId.Value,
                cancellationToken);

            if (fileImage is null)
            {
                logger.LogWarning("FileImage metadata for Medicine {MedicineId} was not found.", request.MedicineId);
                return Error.NotFound(
                    "FileImage.NotFound",
                    "The image metadata was not found.");
            }

            // Open the physical file as a stream to avoid loading the entire image into memory.
            var stream = await fileImageService.GetFileStreamAsync(
                fileImage.StoredFileName,
                cancellationToken);

            if (stream is null)
            {
                logger.LogWarning("Physical image file {StoredFileName} was not found.", fileImage.StoredFileName);
                return Error.NotFound(
                    "FileImage.PhysicalFileNotFound",
                    "The physical image file was not found.");
            }

            return new GetMedicineImageResponse(stream, fileImage.ContentType);
        }
    }
}
