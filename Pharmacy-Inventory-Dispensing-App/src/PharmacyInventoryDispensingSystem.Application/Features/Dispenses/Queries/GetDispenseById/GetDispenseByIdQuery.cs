using MediatR;
using PharmacyInventoryDispensingSystem.Application.Features.Dispenses.Dtos;
using PharmacyInventoryDispensingSystem.Domain.Common.Results;
using System;
using System.Collections.Generic;
using System.Text;

namespace PharmacyInventoryDispensingSystem.Application.Features.Dispenses.Queries.GetDispenseById
{
    public sealed record GetDispenseByIdQuery(Guid DispenseId)
    : IRequest<Result<DispenseDetailsDto>>;
}
