using MediatR;
using PharmacyInventoryDispensingSystem.Application.Features.Medicines.Dtos;
using PharmacyInventoryDispensingSystem.Domain.Common.Results;
using System;
using System.Collections.Generic;
using System.Text;

namespace PharmacyInventoryDispensingSystem.Application.Features.Medicines.Queries.LookupMedicines
{
    public sealed record LookupMedicinesQuery(
     string SearchTerm)
     : IRequest<Result<List<MedicineLookupDto>>>;
}
