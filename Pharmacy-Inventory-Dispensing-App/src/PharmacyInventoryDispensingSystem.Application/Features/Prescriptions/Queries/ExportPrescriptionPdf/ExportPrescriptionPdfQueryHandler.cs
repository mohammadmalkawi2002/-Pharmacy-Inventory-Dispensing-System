using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using PharmacyInventoryDispensingSystem.Application.Common.Interfaces;
using PharmacyInventoryDispensingSystem.Application.Common.Interfaces.Authorization;
using PharmacyInventoryDispensingSystem.Application.Common.Interfaces.Repositories;
using PharmacyInventoryDispensingSystem.Application.Features.Prescriptions.Dtos;
using PharmacyInventoryDispensingSystem.Application.Features.Prescriptions.Mappers;
using PharmacyInventoryDispensingSystem.Domain.Common.Results;
using PharmacyInventoryDispensingSystem.Domain.Entities.Prescriptions;
using System;
using System.Collections.Generic;
using System.Net.Mime;
using System.Text;

namespace PharmacyInventoryDispensingSystem.Application.Features.Prescriptions.Queries.ExportPrescriptionPdf
{
    public sealed class ExportPrescriptionPdfQueryHandler(
        IGenericRepository<Prescription> repository,
        IPrescriptionPdfService prescriptionPdfService,
        IUserLookupService userLookup,
      IPrescriptionAuthorizationService prescriptionAuthorizationService,
      ILogger<ExportPrescriptionPdfQueryHandler> logger)
        : IRequestHandler<ExportPrescriptionPdfQuery, Result<PrescriptionPdfFileResult>>
    {
        public async Task<Result<PrescriptionPdfFileResult>> Handle(
            ExportPrescriptionPdfQuery request,
            CancellationToken cancellationToken)
        {
            // Retrieve the prescription by ID with details(items medicine  and patient information)

            var prescription = await repository.Query()
                                 .Include(p => p.Patient)
                                 .Include(p => p.Items)
                                     .ThenInclude(item => item.Medicine)
                                 .SingleOrDefaultAsync(
                                     p => p.Id == request.PrescriptionId,
                                     cancellationToken);

            if (prescription is null)
            {
                logger.LogWarning(
                    "Prescription with ID {PrescriptionId} not found.",
                    request.PrescriptionId);

                return PrescriptionErrors.NotFound(request.PrescriptionId);
            }

            //Ownership Authorization check:

            bool canAccess = await prescriptionAuthorizationService.CanAccessAsync(
                prescription,
                cancellationToken);


            if (!canAccess)
            {
                logger.LogWarning(
                    "User attempted unauthorized access to prescription {PrescriptionId}.",
                    prescription.Id);

                return PrescriptionErrors.Forbidden;
            }


            // Retrieve the doctor's name using the user lookup service
           var doctorNames = await userLookup.GetUserNamesByIdsAsync(
               [prescription.DoctorId],
               cancellationToken);

            // Get the doctor's name from the dictionary
            doctorNames.TryGetValue(
                prescription.DoctorId,
                out var doctorName);

            //Map to PrescriptionPdfDto:
            var pdfDto=prescription.ToPdfDto(
                doctorName?? "Unknown Doctor");

            // Call pdf service:
            var pdfBytes= prescriptionPdfService.Generate(pdfDto);

            var pdfFileName = $"Prescription-{prescription.PrescriptionNumber}.pdf";
            var contentType = MediaTypeNames.Application.Pdf;

            return new PrescriptionPdfFileResult(
                pdfBytes,
                pdfFileName,
                contentType);

        }
    }
}
