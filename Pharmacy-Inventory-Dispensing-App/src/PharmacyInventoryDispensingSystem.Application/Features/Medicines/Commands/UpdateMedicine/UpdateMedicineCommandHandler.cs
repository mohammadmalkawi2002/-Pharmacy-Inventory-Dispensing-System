using MediatR;
using Microsoft.Extensions.Logging;
using PharmacyInventoryDispensingSystem.Application.Common.Errors;
using PharmacyInventoryDispensingSystem.Application.Common.Interfaces.Repositories;
using PharmacyInventoryDispensingSystem.Domain.Common.Results;
using PharmacyInventoryDispensingSystem.Domain.Entities.Medicines;
using PharmacyInventoryDispensingSystem.Domain.Entities.Patients;
using System;
using System.Reflection.Metadata;
using System.Threading;
using System.Threading.Tasks;

namespace PharmacyInventoryDispensingSystem.Application.Features.Medicines.Commands.UpdateMedicine
{
    public sealed class UpdateMedicineCommandHandler(
        IMedicineRepository medicineRepository,
        IUnitOfWork unitOfWork,
        ILogger<UpdateMedicineCommandHandler> logger)
        : IRequestHandler<UpdateMedicineCommand, Result<Updated>>
    {
        public async Task<Result<Updated>> Handle(
            UpdateMedicineCommand request,
            CancellationToken cancellationToken)
        {
            var medicine = await medicineRepository.GetByIdAsync(
                request.MedicineId,
                trackChanges: true,
                cancellationToken: cancellationToken);

            if (medicine is null)
            {
                logger.LogWarning("Medicine {MedicineId} not found. Update was rejected.", request.MedicineId);
                return MedicineErrors.NotFound(request.MedicineId);
            }

            //→ Check Code Uniqueness
            var code = request.Code.Trim();
            bool codeChanged = !string.Equals(
                medicine.Code,
                code,
                StringComparison.OrdinalIgnoreCase);


            if (codeChanged)
            {
                var codeExists = await medicineRepository.ExistsByCodeAsync(
                    code,
                    cancellationToken);

                if (codeExists)
                {
                    logger.LogWarning(
                        "Medicine update was rejected for {MedicineId} because a medicine with code '{Code}' already exists.",
                        medicine.Id,
                        code);
                    return MedicineErrors.CodeConflict;
                }
            }


            //→ Validate Stock Configuration Change
            var stockConfigurationResult = await ValidateStockConfigurationChangeAsync(medicine,
                                                                                 request,
                                                                                 cancellationToken);


            if (stockConfigurationResult.IsError) 
            {
                logger.LogWarning(
                    "Stock configuration change was rejected for medicine {MedicineId}.",
                    medicine.Id);

                return stockConfigurationResult.Errors;

            }

            //→Apply Changes
            medicine.Code = code;
            medicine.Name = request.Name.Trim();
            medicine.Strength = request.Strength.Trim();
            medicine.Form = request.Form;
            medicine.StockUnit = request.StockUnit;
            medicine.PackageUnit = request.PackageUnit;
            medicine.UnitsPerPackage = request.UnitsPerPackage;
            medicine.ReorderLevel = request.ReorderLevel;

            await unitOfWork.SaveChangesAsync(cancellationToken);

            logger.LogInformation("Medicine {MedicineId} was updated successfully.", medicine.Id);

            return Result.Updated;
        }


        private async Task<Result<Success>> ValidateStockConfigurationChangeAsync(
            Medicine medicine,
            UpdateMedicineCommand request,
            CancellationToken cancellationToken)
        {
            var stockConfigurationChanged =
          medicine.StockUnit != request.StockUnit ||
        medicine.PackageUnit != request.PackageUnit ||
        medicine.UnitsPerPackage != request.UnitsPerPackage;



            if (!stockConfigurationChanged)
                return Result.Success;


            if (medicine.QuantityInStock > 0)
                return MedicineErrors.StockConfigurationCannotBeChanged;

            var isUsedInPrescription =
                await medicineRepository.IsReferencedByPrescriptionAsync(
                    medicine.Id,
                    cancellationToken);




            if (isUsedInPrescription)
                return MedicineErrors.StockConfigurationCannotBeChanged;



            return Result.Success;
           
        }
    }
}
