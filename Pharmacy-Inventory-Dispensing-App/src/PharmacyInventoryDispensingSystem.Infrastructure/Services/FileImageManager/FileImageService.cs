using Microsoft.Extensions.Options;
using PharmacyInventoryDispensingSystem.Application.Common.Files;
using PharmacyInventoryDispensingSystem.Application.Common.Interfaces;
using PharmacyInventoryDispensingSystem.Application.Common.Interfaces.Repositories;
using PharmacyInventoryDispensingSystem.Domain.Common.Results;
using PharmacyInventoryDispensingSystem.Domain.Entities.FileSystem;
using System;
using System.Collections.Generic;
using System.Text;

namespace PharmacyInventoryDispensingSystem.Infrastructure.Services.FileImageManager
{
    public sealed class FileImageService : IFileImageService
    {
        private static readonly HashSet<string> AllowedExtensions = 
            [
                ".jpg",
                ".jpeg",
                ".png",
                ".webp",
            ];


        private readonly IGenericRepository<FileImage> _fileImageRepository;
        private readonly string _folderPath;
        private readonly int _maxFileSizeInBytes;

        public FileImageService(IGenericRepository<FileImage> fileImageRepository,IOptions<FileImageSettings> settings)
        {
            _fileImageRepository = fileImageRepository;
            _maxFileSizeInBytes=settings.Value.MaxFileSizeInMB * 1024 * 1024; //Convert 5MB => 5 * 1024 *1024 bytes 
            _folderPath = Path.Combine(Directory.GetCurrentDirectory(), settings.Value.PathFolder);
        }


        public async Task<Result<FileImage>> UploadAsync(
            UploadedFile file,
            CancellationToken cancellationToken = default)
        {
            if (file.Length == 0) 
            {
                return Error.Validation(
                    code: "FileImage.Empty",
                    description: "File cannot be empty.");

            }


            if (file.Length > _maxFileSizeInBytes)
            {
                return Error.Validation(
                    "FileImage.SizeExceeded",
                    $"File size exceeds the maximum allowed size of " +
                    $"{_maxFileSizeInBytes / (1024 * 1024)} MB.");
            }

            //2. Extract and validate the file extension.

            var extension=Path.GetExtension(file.FileName).ToLowerInvariant();

            if (string.IsNullOrWhiteSpace(extension) || !AllowedExtensions.Contains(extension)) 
            {
                return Error.Validation(
                code: "FileImage.UnsupportedFormat",
                description: "File format is not supported.");

            }

            // 3.Generate a unique physical file name to prevent name conflicts.

            //ex] 7e9f00b01c614a50858d649a824e4349.jpg
            var storedFileName = $"{Guid.NewGuid():N}{extension}";

            //4. Ensure the configured image directory exists.
            Directory.CreateDirectory(_folderPath);

            var filePath = Path.Combine(_folderPath,storedFileName);

            //5. Copy the uploaded stream to the physical image file.

            await using var fileStream=File.Create(filePath);

            await file.Content.CopyToAsync(fileStream, cancellationToken);


            //6. Store the image metadata; the actual bytes remain on the file system.

            var fileImage = new FileImage 
            { 
                  OriginalFileName=file.FileName,
                  StoredFileName=storedFileName,
                  ContentType=file.ContentType,
                  Extension=extension,
                  FileSize=file.Length
            };

            _fileImageRepository.Add(fileImage);

            return fileImage;
        }

        public void Delete(FileImage fileImage)
        {
            var filePath = Path.Combine(
              _folderPath,
              fileImage.StoredFileName);

            if (File.Exists(filePath))
            {
                File.Delete(filePath);
            }

            _fileImageRepository.HardDelete(fileImage);
        }

        public Task<Stream?> GetFileStreamAsync(string storedFileName, CancellationToken cancellationToken = default)
        {
            var filePath = Path.Combine(_folderPath, storedFileName);

            if (!File.Exists(filePath))
            {
                return Task.FromResult<Stream?>(null);
            }

            return Task.FromResult<Stream?>(File.OpenRead(filePath));
        }

        public void DeletePhysicalFile(string storedFileName)
        {
            var filePath = Path.Combine(_folderPath, storedFileName);

            if (File.Exists(filePath))
            {
                File.Delete(filePath);
            }
        }

      
    }
}
