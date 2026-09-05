using Microsoft.EntityFrameworkCore;
using PharmacyInventoryDispensingSystem.Application.Common.Interfaces.Repositories;
using PharmacyInventoryDispensingSystem.Application.Features.Patients.Common;
using PharmacyInventoryDispensingSystem.Application.Features.Patients.Dtos;
using PharmacyInventoryDispensingSystem.Domain.Entities.Patients;
using PharmacyInventoryDispensingSystem.Infrastructure.Persistence.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using static PharmacyInventoryDispensingSystem.Application.Features.SecurityManager.Authorization.Permissions;

namespace PharmacyInventoryDispensingSystem.Infrastructure.Persistence.Repositories
{
    public class PatientRepository(AppDbContext context) : IPatientRepository
    {
        public async Task AddAsync(
            Patient patient,
            CancellationToken cancellationToken = default)
        {
            await context.Patients.AddAsync(
                patient,
                cancellationToken);
           
        }



        /// <summary>
        /// this method used in frontend to use when selelct  Patients from dropdown in CreatePrescription
        /// </summary>
        /// <param name="searchTerm">by DocumentId Or FullName</param>
        /// <param name="limit">the number of paitents returned</param>
        /// <param name="cancellationToken"></param>
        /// <returns>Active  Patientss </returns>

        public async Task<List<PatientLookupDto>> SearchForLookupAsync(
            string searchTerm,
            int limit,
            CancellationToken cancellationToken = default)
        {
            string normalizedSearchTerm = searchTerm.Trim();

            return await context.Patients
                .AsNoTracking()
                .Where(patient =>
                    patient.FullName.Contains(normalizedSearchTerm) ||
                    patient.DocumentId.StartsWith(normalizedSearchTerm))
                .OrderBy(patient => patient.FullName)
                .ThenBy(patient => patient.Id)
                .Take(limit)
                .Select(patient=>new PatientLookupDto(patient.Id,patient.DocumentId,patient.FullName))
                .ToListAsync(cancellationToken);
        }



        public async Task<bool> ExistsByDocumentIdAsync(
            string documentId,
           
            CancellationToken cancellationToken = default)
        {
            return await context.Patients
                        .IgnoreQueryFilters()
                        .AnyAsync(
                patient => patient.DocumentId == documentId,
                cancellationToken);


        }

        public async Task<Patient?> GetByDocumentIdAsync(
            string documentId,
            CancellationToken cancellationToken = default)
        {

            return await context.Patients
                .AsNoTracking()
                .SingleOrDefaultAsync(Patient => Patient.DocumentId == documentId);
        }

        public async Task<Patient?> GetByIdAsync(
            Guid patientId,
            bool trackChanges = false,
            CancellationToken cancellationToken = default)
        {
            var query = context.Patients.AsQueryable();

            if (!trackChanges) 
            {
                query=query.AsNoTracking();
            }
            return await query.FirstOrDefaultAsync(
                Patient => Patient.Id == patientId, 
                cancellationToken);

        }

        public async Task<(IReadOnlyList<Patient> Items, int TotalCount)> GetPagedAsync(
            string? searchTerm,
            PatientDocumentType? documentType,
            string? sortBy,
            bool isDescending,
            int pageNumber,
            int pageSize,
            CancellationToken cancellationToken = default)
        {

            var query = context.Patients.AsNoTracking();

           
            query = ApplyFiltering(
                query,
                documentType);

            

            query= ApplySearch(
                query,
                searchTerm);

            
            int totalCount = await query.CountAsync(cancellationToken);

            IOrderedQueryable<Patient> orderedQuery=ApplySorting(
                query,
                sortBy,
                isDescending);

          IReadOnlyList<Patient> patients= await orderedQuery
                    .Skip((pageNumber - 1) * pageSize)
                    .Take(pageSize)
                    .ToListAsync(cancellationToken);

            return (patients,totalCount);


            
        }

        private static IOrderedQueryable<Patient> ApplySorting(
            IQueryable<Patient> query,
            string? sortBy,
            bool isDescending)
        {
            string normalizedSortBy = sortBy?.Trim().ToLower() ?? "createdatutc";

            return normalizedSortBy switch
            {
                "fullname" => isDescending
                    ? query
                        .OrderByDescending(patient => patient.FullName)
                        .ThenByDescending(patient => patient.Id)
                    : query
                        .OrderBy(patient => patient.FullName)
                        .ThenBy(patient => patient.Id),

                "createdatutc" => isDescending
                    ? query
                        .OrderByDescending(patient => patient.CreatedAtUtc)
                        .ThenByDescending(patient => patient.Id)
                    : query
                        .OrderBy(patient => patient.CreatedAtUtc)
                        .ThenBy(patient => patient.Id),

                _ => query
                    .OrderByDescending(patient => patient.CreatedAtUtc)
                    .ThenByDescending(patient => patient.Id)
            };
           
        }

        private static IQueryable<Patient> ApplySearch(
            IQueryable<Patient> query,
            string? searchTerm)
        {
            if (string.IsNullOrWhiteSpace(searchTerm)) 
            {
                return query;
            }


            string normalizedSearchTerm = searchTerm.Trim();

            return query.Where(patient =>
                 patient.DocumentId.StartsWith(normalizedSearchTerm) ||
                 patient.FullName.StartsWith(normalizedSearchTerm) ||
                 patient.PhoneNumber.StartsWith(normalizedSearchTerm));


        }

        private static IQueryable<Patient> ApplyFiltering(
            IQueryable<Patient> query,
            PatientDocumentType? documentType) 
        {
            return documentType switch
            {
                PatientDocumentType.Citizen =>
                          query.Where(patient => 
                            patient.DocumentId.StartsWith("1")),


                    PatientDocumentType.Resident=>
                    query.Where(patient=> patient.DocumentId.StartsWith("2")),

                    _=>query

            };
        
        }

        public async Task<Patient?> GetByIdIncludingArchivedAsync(
            Guid patientId,
            CancellationToken cancellationToken = default)
        {
            return await context.Patients
             .IgnoreQueryFilters()
             .FirstOrDefaultAsync(
                 patient => patient.Id == patientId,
                 cancellationToken);
        }

        public async Task<(IReadOnlyList<Patient> patients, int TotalCount)> GetArchivedPagedAsync(string? searchTerm, PatientDocumentType? documentType, string? sortBy, bool isDescending, int pageNumber, int pageSize, CancellationToken cancellationToken = default)
        {
            var query = context.Patients
                        .IgnoreQueryFilters()
                         .AsNoTracking()
                         .Where(patient=>patient.IsDeleted);

            query = ApplyFiltering(query, documentType);

            query = ApplySearch(query, searchTerm);

            int totalCount = await query.CountAsync(cancellationToken);

            IOrderedQueryable<Patient> orderedQuery = ApplySorting(query, sortBy, isDescending);

            List<Patient> patients = await orderedQuery
                                     .Skip((pageNumber - 1) * pageSize)
                                      .Take(pageSize)
                                    .ToListAsync(cancellationToken);


            return (patients, totalCount);




        }
    }
}
