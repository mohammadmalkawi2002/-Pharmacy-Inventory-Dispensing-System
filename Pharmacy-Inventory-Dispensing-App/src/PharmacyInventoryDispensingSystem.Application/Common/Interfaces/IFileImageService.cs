using PharmacyInventoryDispensingSystem.Application.Common.Files;
using PharmacyInventoryDispensingSystem.Domain.Common.Results;
using PharmacyInventoryDispensingSystem.Domain.Entities.FileSystem;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace PharmacyInventoryDispensingSystem.Application.Common.Interfaces
{
    public interface IFileImageService
    {
        Task<Result<FileImage>> UploadAsync(
         UploadedFile file,
         CancellationToken cancellationToken = default);

        Task<Stream?> GetFileStreamAsync(
            string storedFileName,
            CancellationToken cancellationToken = default);

        void Delete(FileImage fileImage);

        void DeletePhysicalFile(string storedFileName);


    }
}
