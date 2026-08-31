using MediatR;
using Microsoft.Extensions.Logging;
using PharmacyInventoryDispensingSystem.Application.Common.Interfaces.Repositories;
using PharmacyInventoryDispensingSystem.Application.Features.Medicines.Dtos;
using PharmacyInventoryDispensingSystem.Application.Features.Medicines.Mappers;
using PharmacyInventoryDispensingSystem.Domain.Common.Results;
using PharmacyInventoryDispensingSystem.Domain.Entities.Medicines;
using System.Threading;
using System.Threading.Tasks;

namespace PharmacyInventoryDispensingSystem.Application.Features.Medicines.Queries.GetMedicineByCode
{
    public sealed class GetMedicineByCodeQueryHandler(
        IMedicineRepository medicineRepository,
        ILogger<GetMedicineByCodeQueryHandler> logger)
        : IRequestHandler<GetMedicineByCodeQuery, Result<MedicineDetailsResponseDto>>
    {
        public async Task<Result<MedicineDetailsResponseDto>> Handle(
            GetMedicineByCodeQuery request,
            CancellationToken cancellationToken)
        {
            var code = request.Code.Trim();

            var medicine = await medicineRepository.GetByCodeAsync(
                code,
                cancellationToken);

            if (medicine is null)
            {
                logger.LogWarning("Medicine with code '{Code}' was not found.", code);
                return MedicineErrors.NotFoundByCode(code);
            }

            return medicine.ToDetailsDto();
        }
    }
}
