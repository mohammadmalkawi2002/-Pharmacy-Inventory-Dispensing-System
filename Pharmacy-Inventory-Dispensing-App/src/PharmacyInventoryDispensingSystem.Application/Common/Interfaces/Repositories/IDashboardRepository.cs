using System;
using System.Collections.Generic;
using System.Text;

namespace PharmacyInventoryDispensingSystem.Application.Common.Interfaces.Repositories
{
    public interface IDashboardRepository
    {
        Task<int> CountPatientsAsync(
            CancellationToken cancellationToken = default);

        Task<int> CountMedicinesAsync(
            CancellationToken cancellationToken = default);

        Task<int> CountLowStockMedicinesAsync(
            CancellationToken cancellationToken = default);

        Task<int> CountActivePrescriptionsAsync(
            DateOnly today,
            string? doctorId,
            CancellationToken cancellationToken = default);

        Task<int> CountRecentDispensesAsync(
            DateTimeOffset fromInclusive,
            DateTimeOffset toExclusive,
            string? performedByUserId,
            CancellationToken cancellationToken = default);
    }
}
