using MediatR;
using PharmacyInventoryDispensingSystem.Application.Common.Interfaces;
using PharmacyInventoryDispensingSystem.Application.Common.Interfaces.Repositories;
using PharmacyInventoryDispensingSystem.Application.Features.Dashboard.Dtos;
using PharmacyInventoryDispensingSystem.Application.Features.SecurityManager.Authorization;
using PharmacyInventoryDispensingSystem.Domain.Common.Results;
using System;
using System.Collections.Generic;
using System.Text;

namespace PharmacyInventoryDispensingSystem.Application.Features.Dashboard.Queries
{
    public sealed class GetDashboardSummaryQueryHandler(
     IDashboardRepository dashboardRepository,
     ICurrentUser currentUser)
     : IRequestHandler<
         GetDashboardSummaryQuery,
         Result<DashboardSummaryDto>>
    {
        public async Task<Result<DashboardSummaryDto>> Handle(
            GetDashboardSummaryQuery query,
            CancellationToken cancellationToken)
        {
            string? userId = currentUser.Id;

            if (string.IsNullOrWhiteSpace(userId))
            {
                return Error.Unauthorized(
                    "Dashboard.UserNotIdentified",
                    "The authenticated user could not be identified.");
            }

            bool isAdmin = currentUser.IsInRole(RoleNames.Admin);
            bool isDoctor = currentUser.IsInRole(RoleNames.Doctor);
            bool isPharmacist = currentUser.IsInRole(RoleNames.Pharmacist);

            int? totalPatients = null;
            int? totalMedicines = null;
            int? lowStockMedicines = null;
            int? activePrescriptions = null;
            int? recentDispenses = null;

            // Patient statistics: Admin, Receptionist, and Doctor.
            if (currentUser.HasPermission(Permissions.Patients.Read))
            {
                totalPatients =
                    await dashboardRepository.CountPatientsAsync(
                        cancellationToken);
            }

            // Medicine statistics: Admin, Doctor, and Pharmacist.
            if (currentUser.HasPermission(Permissions.Medicines.Read))
            {
                totalMedicines =
                    await dashboardRepository.CountMedicinesAsync(
                        cancellationToken);
            }

            // Low-stock statistics: Admin and Pharmacist.
            if (currentUser.HasPermission(
                    Permissions.Medicines.ReadLowStock))
            {
                lowStockMedicines =
                    await dashboardRepository.CountLowStockMedicinesAsync(
                        cancellationToken);
            }

            // Active prescriptions: Admin sees all; Doctor sees their own.
            if (currentUser.HasPermission(Permissions.Prescriptions.Read) &&
                (isAdmin || isDoctor))
            {
                DateOnly today =
                    DateOnly.FromDateTime(DateTime.UtcNow);

                string? doctorId = isAdmin ? null : userId;

                activePrescriptions =
                    await dashboardRepository.CountActivePrescriptionsAsync(
                        today,
                        doctorId,
                        cancellationToken);
            }

            // Recent dispenses means today's dispensing operations.
            // Admin sees all; Pharmacist sees only their own.
            if (currentUser.HasPermission(Permissions.Dispenses.Read) &&
                (isAdmin || isPharmacist))
            {
                DateTimeOffset now = DateTimeOffset.UtcNow;

                DateTimeOffset startOfTodayUtc = new(
                    now.Year,
                    now.Month,
                    now.Day,
                    0,
                    0,
                    0,
                    TimeSpan.Zero);

                DateTimeOffset startOfTomorrowUtc =
                    startOfTodayUtc.AddDays(1);

                string? performedByUserId =
                    isAdmin ? null : userId;

                recentDispenses =
                    await dashboardRepository.CountRecentDispensesAsync(
                        startOfTodayUtc,
                        startOfTomorrowUtc,
                        performedByUserId,
                        cancellationToken);
            }

            return new DashboardSummaryDto(
                totalPatients,
                totalMedicines,
                lowStockMedicines,
                activePrescriptions,
                recentDispenses);
        }
    }

}