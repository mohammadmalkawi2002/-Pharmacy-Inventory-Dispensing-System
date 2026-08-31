using MediatR;
using PharmacyInventoryDispensingSystem.Application.Features.Medicines.Dtos;
using PharmacyInventoryDispensingSystem.Domain.Common.Results;
using System;
using System.Collections.Generic;
using System.Text;

namespace PharmacyInventoryDispensingSystem.Application.Features.Medicines.Commands.ReceiveStock
{
    public sealed record ReceiveStockCommand(
        Guid MedicineId,
        int PackageQuantity):IRequest<Result<ReceiveStockResponseDto>>;
   
}
