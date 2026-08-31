using PharmacyInventoryDispensingSystem.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace PharmacyInventoryDispensingSystem.Application.Features.Medicines.Dtos
{
    public sealed record ReceiveStockResponseDto(
    Guid MedicineId,
    int ReceivedPackages,
    PackageUnit PackageUnit,
    int ReceivedQuantity,
    StockUnit StockUnit,
    int OldQuantity,
    int NewQuantity);
}
