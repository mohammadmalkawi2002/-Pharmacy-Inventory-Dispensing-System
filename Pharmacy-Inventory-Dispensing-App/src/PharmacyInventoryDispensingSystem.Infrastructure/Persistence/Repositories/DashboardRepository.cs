using Microsoft.EntityFrameworkCore;
using PharmacyInventoryDispensingSystem.Application.Common.Interfaces.Repositories;
using PharmacyInventoryDispensingSystem.Domain.Entities.Prescriptions;
using PharmacyInventoryDispensingSystem.Domain.Enums;
using PharmacyInventoryDispensingSystem.Infrastructure.Persistence.Context;
using System;
using System.Collections.Generic;
using System.Text;

namespace PharmacyInventoryDispensingSystem.Infrastructure.Persistence.Repositories
{
    public sealed class DashboardRepository(AppDbContext context)
    : IDashboardRepository
    {
        public async Task<int> CountPatientsAsync(CancellationToken cancellationToken = default)
        {
            // The global query filter excludes archived patients.
            return await context.Patients.CountAsync(cancellationToken);
        }

        public async Task<int> CountMedicinesAsync(CancellationToken cancellationToken = default)
        {
            // The global query filter excludes archived medicines.
            return await context.Medicines.CountAsync(cancellationToken);
        }

        public async Task<int> CountActivePrescriptionsAsync(DateOnly today, string? doctorId, CancellationToken cancellationToken = default)
        {
            var query = context.Prescriptions
             .Where(prescription =>
                 prescription.Status == PrescriptionStatus.Active &&
                 prescription.ValidFrom <= today &&
                 prescription.ValidTo >= today);

            // Doctor sees only their prescriptions.
            // Admin passes null and sees all prescriptions.
            if (!string.IsNullOrWhiteSpace(doctorId))
            {
                query = query.Where(
                    prescription => prescription.DoctorId == doctorId);
            }

            return await query.CountAsync(cancellationToken);

        }

        public async Task<int> CountLowStockMedicinesAsync(CancellationToken cancellationToken = default)
        { 

            return await context.Medicines.CountAsync(
            medicine =>
                medicine.QuantityInStock > 0 &&
                medicine.QuantityInStock <= medicine.ReorderLevel,
            cancellationToken);
        }

       

        

        public async Task<int> CountRecentDispensesAsync(DateTimeOffset fromInclusive, DateTimeOffset toExclusive, string? performedByUserId, CancellationToken cancellationToken = default)
        {
            var query = context.Dispenses
           .Where(dispense =>
               dispense.DispensedAt >= fromInclusive &&
               dispense.DispensedAt < toExclusive);

            // Pharmacist sees their dispensing operations.
            // Admin passes null and sees all dispensing operations.
            if (!string.IsNullOrWhiteSpace(performedByUserId))
            {
                query = query.Where(
                    dispense =>
                        dispense.PharmacistId == performedByUserId);
            }

            return await query.CountAsync(cancellationToken);
        }
    }
}