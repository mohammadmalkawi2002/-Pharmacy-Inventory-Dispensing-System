using Microsoft.EntityFrameworkCore;
using PharmacyInventoryDispensingSystem.Application.Features.Patients.Common;
using PharmacyInventoryDispensingSystem.Domain.Entities.Patients;
using System;
using System.Collections.Generic;
using System.Text;

namespace PharmacyInventoryDispensingSystem.Application.Common.Interfaces.Repositories
{
    public interface IPatientRepository
    {
        Task AddAsync(
            Patient patient,
            CancellationToken cancellationToken = default);

        Task<Patient?> GetByIdAsync(
        Guid patientId,
         bool trackChanges = false,
        CancellationToken cancellationToken = default);


           Task<Patient?> GetByDocumentIdAsync(
        string documentId,
        CancellationToken cancellationToken = default);

        Task<bool> ExistsByDocumentIdAsync(
            string documentId,
            CancellationToken cancellationToken = default);

        Task<(IReadOnlyList<Patient> Items,int TotalCount)> GetPagedAsync(
            string? searchTerm,
            PatientDocumentType? documentType,
            string? sortBy,
            bool isDescending,
            int pageNumber,
            int pageSize ,
            CancellationToken cancellationToken = default);


        Task<Patient?> GetByIdIncludingArchivedAsync(
        Guid patientId,
        CancellationToken cancellationToken = default);
       

        Task<(IReadOnlyList<Patient> patients,int TotalCount)> GetArchivedPagedAsync(
            string? searchTerm,
            PatientDocumentType? documentType,
            string? sortBy,
            bool isDescending,
            int pageNumber,
            int pageSize,
            CancellationToken cancellationToken = default);







    }
}
