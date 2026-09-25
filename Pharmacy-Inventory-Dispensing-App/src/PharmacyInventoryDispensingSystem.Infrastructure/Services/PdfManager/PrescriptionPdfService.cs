using Microsoft.Extensions.Logging;
using PharmacyInventoryDispensingSystem.Application.Common.Interfaces;
using PharmacyInventoryDispensingSystem.Application.Features.Prescriptions.Dtos;
using QuestPDF.Fluent;
namespace PharmacyInventoryDispensingSystem.Infrastructure.Services.PdfManager
{
    /// <summary>
    ///  Generate/orchestration: receives PrescriptionPdfDto ,then create instance of PrescriptionPdfDocument(QuestPDFLayout)
    /// </summary>
    public sealed class PrescriptionPdfService(ILogger<PrescriptionPdfService> logger) : IPrescriptionPdfService
    {
        public byte[] Generate(PrescriptionPdfDto prescriptionDto)
        {
            logger.LogInformation(
            "Starting PDF generation for prescription {PrescriptionNumber}",
            prescriptionDto.PrescriptionNumber);

            var document = new PrescriptionPdfDocument(prescriptionDto);

            var pdfBytes = document.GeneratePdf();


            logger.LogInformation(
                "PDF generation completed successfully for prescription {PrescriptionNumber}",
                prescriptionDto.PrescriptionNumber);

            return pdfBytes;


        }
    }
}
