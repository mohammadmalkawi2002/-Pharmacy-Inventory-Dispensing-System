using MediatR;
using Microsoft.Extensions.Logging;
using PharmacyInventoryDispensingSystem.Application.Common.Interfaces.Repositories;
using PharmacyInventoryDispensingSystem.Application.Features.Medicines.Dtos;
using PharmacyInventoryDispensingSystem.Domain.Common.Results;
using PharmacyInventoryDispensingSystem.Domain.Entities.Medicines;
using System;
using System.Collections.Generic;
using System.Text;

namespace PharmacyInventoryDispensingSystem.Application.Features.Medicines.Commands.ReceiveStock
{
    public sealed class ReceiveStockCommandHandler(
     IMedicineRepository medicineRepository,
     IUnitOfWork unitOfWork,
     ILogger<ReceiveStockCommandHandler> logger)
     : IRequestHandler<ReceiveStockCommand, Result<ReceiveStockResponseDto>>
    {
        public async Task<Result<ReceiveStockResponseDto>> Handle(ReceiveStockCommand request, CancellationToken cancellationToken)
        {
            var medicine = await medicineRepository.GetByIdAsync(
                request.MedicineId,
                trackChanges: true,
                cancellationToken);



            if (medicine is null)
            {
                logger.LogWarning(
                    "Medicine {MedicineId} was not found. Stock receiving rejected.",
                    request.MedicineId);

                return MedicineErrors.NotFound(request.MedicineId);
            }


            if(!medicine.IsActive)
            {
                logger.LogWarning(
               "Medicine {MedicineId} is inactive. Stock receiving rejected.",
               request.MedicineId);

                return MedicineErrors.Inactive(medicine.Code);
            }
            var oldQuantity = medicine.QuantityInStock;
            var receivedQuantity = request.PackageQuantity * medicine.UnitsPerPackage;

            var increaseStockResult=medicine.IncreaseStock(receivedQuantity);

            if (increaseStockResult.IsError) 
            {

                return increaseStockResult.TopError;
            }
            await unitOfWork.SaveChangesAsync(cancellationToken);


            logger.LogInformation(
         "Received {PackageQuantity} {PackageUnit} of medicine {MedicineId}, adding {ReceivedQuantity} {StockUnit} to stock.",
         request.PackageQuantity,
         medicine.PackageUnit,
         medicine.Id,
         receivedQuantity,
         medicine.StockUnit);


            return new ReceiveStockResponseDto(
                medicine.Id,
                request.PackageQuantity,
                medicine.PackageUnit,
                receivedQuantity,
                medicine.StockUnit,
                oldQuantity,
                medicine.QuantityInStock);

        }
    }
}
