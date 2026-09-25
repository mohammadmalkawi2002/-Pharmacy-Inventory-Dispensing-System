using MediatR;
using PharmacyInventoryDispensingSystem.Application.Features.Prescriptions.Dtos;
using PharmacyInventoryDispensingSystem.Domain.Common.Results;
using System;
using System.Collections.Generic;
using System.Text;

namespace PharmacyInventoryDispensingSystem.Application.Features.Prescriptions.Queries.ExportPrescriptionPdf
{
    public sealed record ExportPrescriptionPdfQuery(Guid PrescriptionId)
        : IRequest<Result<PrescriptionPdfFileResult>>;
   
}
