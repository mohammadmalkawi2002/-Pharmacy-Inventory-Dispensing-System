using Microsoft.EntityFrameworkCore;
using PharmacyInventoryDispensingSystem.Application.Common.Interfaces.Repositories;
using PharmacyInventoryDispensingSystem.Domain.Entities.Prescriptions;
using PharmacyInventoryDispensingSystem.Domain.Enums;
using PharmacyInventoryDispensingSystem.Infrastructure.Persistence.Context;
using System;
using System.Collections.Generic;
using System.Text;
using static PharmacyInventoryDispensingSystem.Application.Features.SecurityManager.Authorization.Permissions;

namespace PharmacyInventoryDispensingSystem.Infrastructure.Persistence.Repositories
{
    public class PrescriptionRepository(AppDbContext context) : IPrescriptionRepository
    {
        public async Task AddAsync(Prescription prescription, CancellationToken cancellationToken = default)
        {
          await  context.Prescriptions
                .AddAsync(prescription, cancellationToken);

        }

        public async Task<string> GenerateNextPrescriptionNumberAsync(CancellationToken cancellationToken = default)
        {
            int sequenceValue=await context.Database
                .SqlQuery<int>($"SELECT NEXT VALUE FOR PrescriptionNumberSequence AS Value")
                .AsAsyncEnumerable()
                .SingleAsync(cancellationToken);

            return $"RX-{sequenceValue:D6}";

        }


        public async Task<Prescription?> GetForDispensingAsync(
            Guid prescriptionId,
            string documentId,
            CancellationToken cancellationToken = default)
        {
            return await context.Prescriptions
                .Include(prescription => prescription.Patient)
                .Include(prescription => prescription.Items)
                    .ThenInclude(item => item.Medicine)
                .SingleOrDefaultAsync(
                    prescription =>
                        prescription.Id == prescriptionId &&
                        prescription.Patient.DocumentId == documentId,
                    cancellationToken);
        }
        public async Task<Prescription?> GetByIdWithDetailsAsync(Guid prescriptionId, CancellationToken cancellationToken = default)
        {
            return await context.Prescriptions
                .AsNoTracking()
                .Include(prescription=>prescription.Patient)
                .Include(prescription=>prescription.Items)
                .ThenInclude(item => item.Medicine)
                .SingleOrDefaultAsync(
                prescription => prescription.Id == prescriptionId,
                cancellationToken);
        }

        public async Task<Prescription?> GetByIdAsync(Guid prescriptionId, CancellationToken cancellationToken = default)
        {
            return await context.Prescriptions
             .Include(prescription => prescription.Items)
             .SingleOrDefaultAsync(
                 prescription => prescription.Id == prescriptionId,
                  cancellationToken);

        }




        public async Task<Prescription?> LookupAsync(
            string prescriptionNumber,
            string documentId,
            CancellationToken cancellationToken = default)
        {
            return await context.Prescriptions
                .AsNoTracking()
                .Include(prescription => prescription.Patient)
                .Include(prescription => prescription.Items)
                .ThenInclude(item => item.Medicine)
                .SingleOrDefaultAsync(
                    prescription => 
                    prescription.PrescriptionNumber == prescriptionNumber &&
                    prescription.Patient.DocumentId == documentId,
                    cancellationToken);
        }

        public async Task<Prescription?> GetByIdForCancellationAsync(
            Guid prescriptionId,
            CancellationToken cancellationToken = default)
        {
            return await context.Prescriptions
                .SingleOrDefaultAsync(
                    prescription => prescription.Id == prescriptionId,
                    cancellationToken);
        }








        public async Task<(IReadOnlyList<Prescription> Items, int TotalCount)> GetPagedAsync(
            string? searchTerm,
            PrescriptionStatus? status,
            string? doctorId,
            string? sortBy,
            bool isDescending,
            int pageNumber,
            int pageSize,
            CancellationToken cancellationToken = default)
        {
            IQueryable<Prescription> query = context.Prescriptions
                .AsNoTracking()
                .Include(prescription => prescription.Patient);


            
            // Doctor ownership filter
            if (!string.IsNullOrWhiteSpace(doctorId))
            {
                query = query.Where(
                    prescription => prescription.DoctorId == doctorId);
            }

            // Status filter
            if (status.HasValue)
            {
                query = query.Where(
                    prescription => prescription.Status == status.Value);
            }

            // Search
            query = ApplySearch(query, searchTerm);

            // Count after filtering/searching, before pagination
            int totalCount = await query.CountAsync(cancellationToken);

            // Sorting
            IOrderedQueryable<Prescription> orderedQuery = ApplySorting(
                query,
                sortBy,
                isDescending);

            // Pagination
            IReadOnlyList<Prescription> prescriptions = await orderedQuery
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync(cancellationToken);

            return (prescriptions, totalCount);



            
        }




        private static IQueryable<Prescription> ApplySearch(
            IQueryable<Prescription> query,
            string? searchTerm)
        {
            if (string.IsNullOrWhiteSpace(searchTerm))
            {
                return query;
            }

            string normalizedSearchTerm = searchTerm.Trim();

            return query.Where(prescription =>
                prescription.PrescriptionNumber.StartsWith(normalizedSearchTerm) ||
                prescription.Patient.FullName.Contains(normalizedSearchTerm) ||
                prescription.Patient.DocumentId.StartsWith(normalizedSearchTerm));
        }


        private static IOrderedQueryable<Prescription> ApplySorting(
            IQueryable<Prescription> query,
            string? sortBy,
            bool isDescending)
        {
            string normalizedSortBy =
                sortBy?.Trim().ToLower() ?? "createdatutc";

            return normalizedSortBy switch
            {
                "prescriptionnumber" => isDescending
                    ? query
                        .OrderByDescending(prescription => prescription.PrescriptionNumber)
                        .ThenByDescending(prescription => prescription.Id)
                    : query
                        .OrderBy(prescription => prescription.PrescriptionNumber)
                        .ThenBy(prescription => prescription.Id),

                "validfrom" => isDescending
                    ? query
                        .OrderByDescending(prescription => prescription.ValidFrom)
                        .ThenByDescending(prescription => prescription.Id)
                    : query
                        .OrderBy(prescription => prescription.ValidFrom)
                        .ThenBy(prescription => prescription.Id),

                "validto" => isDescending
                    ? query
                        .OrderByDescending(prescription => prescription.ValidTo)
                        .ThenByDescending(prescription => prescription.Id)
                    : query
                        .OrderBy(prescription => prescription.ValidTo)
                        .ThenBy(prescription => prescription.Id),

                "createdatutc" => isDescending
                    ? query
                        .OrderByDescending(prescription => prescription.CreatedAtUtc)
                        .ThenByDescending(prescription => prescription.Id)
                    : query
                        .OrderBy(prescription => prescription.CreatedAtUtc)
                        .ThenBy(prescription => prescription.Id),

                _ => query
                    .OrderByDescending(prescription => prescription.CreatedAtUtc)
                    .ThenByDescending(prescription => prescription.Id)
            };
        }

        public void RemoveItem(PrescriptionItem item)
        {
            context.PrescriptionItems.Remove(item);
        }
    }
}
