using System;
using System.Collections.Generic;
using System.Text;

namespace PharmacyInventoryDispensingSystem.Application.Features.Dashboard.Dtos
{
    /// <summary>
    /// Represents dashboard statistics available to the authenticated user.
    /// Unauthorized statistics remain null and are omitted from the JSON response.
    /// </summary>
    public sealed record DashboardSummaryDto(
        int? TotalPatients,
        int? TotalMedicines,
        int? LowStockMedicines,
        int? ActivePrescriptions,
        int? RecentDispenses);
}
