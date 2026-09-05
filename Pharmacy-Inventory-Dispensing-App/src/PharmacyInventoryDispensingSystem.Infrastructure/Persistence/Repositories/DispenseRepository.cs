using Microsoft.EntityFrameworkCore;
using PharmacyInventoryDispensingSystem.Application.Common.Interfaces.Repositories;
using PharmacyInventoryDispensingSystem.Domain.Entities.Dispenses;
using PharmacyInventoryDispensingSystem.Infrastructure.Persistence.Context;
using System;
using System.Collections.Generic;
using System.Text;

namespace PharmacyInventoryDispensingSystem.Infrastructure.Persistence.Repositories
{
    public class DispenseRepository(AppDbContext context) : IDispenseRepository
    {
        public async Task<bool> ExistsForPrescriptionAsync(Guid prescriptionId, CancellationToken cancellationToken)
        {
            return await context.Dispenses
                .AsNoTracking()
                .AnyAsync(d => 
                d.PrescriptionId == prescriptionId,
                cancellationToken);
        }



        public async Task AddAsync(
            Dispense dispense,
            CancellationToken cancellationToken = default)
        {
            await context.Dispenses.AddAsync(
                dispense,
                cancellationToken);
        }

        public async Task<Dispense?> GetByIdWithDetailsAsync(
            Guid dispenseId,
            CancellationToken cancellationToken = default)
        {
            return await context.Dispenses
                .AsNoTracking()
                .Include(dispense => dispense.Prescription)
                    .ThenInclude(prescription => prescription.Patient)
                .Include(dispense => dispense.Items)
                    .ThenInclude(item => item.PrescriptionItem)
                        .ThenInclude(item => item.Medicine)
                .SingleOrDefaultAsync(
                    dispense => dispense.Id == dispenseId,
                    cancellationToken);
        }

        public async Task<(IReadOnlyList<Dispense> Items, int TotalCount)> GetPagedAsync(
            string? searchTerm,
            DateOnly? fromDate,
            DateOnly? toDate,
            int pageNumber,
            int pageSize,
            CancellationToken cancellationToken = default)
        {
            IQueryable<Dispense> query = context.Dispenses
                .AsNoTracking();

            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                string normalizedSearchTerm = searchTerm.Trim();

                query = query.Where(dispense =>
                    dispense.Prescription.PrescriptionNumber
                        .StartsWith(normalizedSearchTerm) ||
                    dispense.Prescription.Patient.FullName
                        .Contains(normalizedSearchTerm) ||
                    dispense.Prescription.Patient.DocumentId
                        .StartsWith(normalizedSearchTerm));
            }

            if (fromDate.HasValue)
            {
                DateTimeOffset from = new(
                    fromDate.Value.ToDateTime(TimeOnly.MinValue),
                    TimeSpan.Zero);

                query = query.Where(
                    dispense => dispense.DispensedAt >= from);
            }

            if (toDate.HasValue)
            {
                DateTimeOffset toExclusive = new(
                    toDate.Value
                        .AddDays(1)
                        .ToDateTime(TimeOnly.MinValue),
                    TimeSpan.Zero);

                query = query.Where(
                    dispense => dispense.DispensedAt < toExclusive);
            }

            int totalCount = await query.CountAsync(cancellationToken);

            IReadOnlyList<Dispense> dispenses = await query
                .Include(dispense => dispense.Prescription)
                    .ThenInclude(prescription => prescription.Patient)
                .OrderByDescending(dispense => dispense.DispensedAt)
                .ThenByDescending(dispense => dispense.Id)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync(cancellationToken);

            return (dispenses, totalCount);
        }

    }
}
