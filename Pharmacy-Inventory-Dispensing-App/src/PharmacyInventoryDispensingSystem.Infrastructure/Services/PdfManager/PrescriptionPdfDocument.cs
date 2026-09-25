using PharmacyInventoryDispensingSystem.Application.Features.Prescriptions.Dtos;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using System;
using System.Collections.Generic;
using System.Text;

namespace PharmacyInventoryDispensingSystem.Infrastructure.Services.PdfManager
{
   
    public class PrescriptionPdfDocument(PrescriptionPdfDto prescription) : IDocument
    {
        //Generated/printed At time
        private readonly DateTimeOffset generatedAt = DateTimeOffset.UtcNow;
        private const string PrimaryColor = "#0284C7";
        private const string TextColor = "#0F172A";
        private const string MutedColor = "#64748B";
        private const string SurfaceColor = "#F8FAFC";
        private const string BorderColor = "#E2E8F0";
        public DocumentMetadata GetMetadata() => DocumentMetadata.Default;
        public void Compose(IDocumentContainer container)
        {
            container.Page(page => 
            {
                page.Size(PageSizes.A4);
                page.Margin(40);


                page.Header().Element(ComposeHeader);

                page.Content().Element(ComposeContent);

                page.Footer().Element(ComposeFooter);


            });
        }


        private void ComposeHeader(IContainer container)
        {
            container
            .PaddingBottom(20)
            .Row(row =>
            {
                row.RelativeItem()
                    .Row(brand =>
                    {
                        brand.ConstantItem(38)
                            .Height(38)
                            .Image(PdfResources.Logo);

                        brand.RelativeItem()
                            .PaddingLeft(10)
                            .Column(column =>
                            {
                                column.Item()
                                    .Text(text =>
                                    {
                                        text.Span("Pharma")
                                            .FontSize(18)
                                            .Bold()
                                            .FontColor(TextColor);

                                        text.Span("Care")
                                            .FontSize(18)
                                            .Bold()
                                            .FontColor(PrimaryColor);
                                    });

                                column.Item()
                                    .PaddingTop(2)
                                    .Text("Smart Inventory & Dispensing")
                                    .FontSize(9)
                                    .FontColor(MutedColor);
                            });
                    });

                row.ConstantItem(150)
                    .AlignRight()
                    .Column(column =>
                    {
                        column.Item()
                            .AlignRight()
                            .Text("PRESCRIPTION")
                            .FontSize(14)
                            .Bold()
                            .FontColor(PrimaryColor);

                        column.Item()
                            .PaddingTop(3)
                            .AlignRight()
                            .Text(prescription.PrescriptionNumber)
                            .FontSize(11)
                            .Bold()
                            .FontColor(TextColor);

                        column.Item()
                            .PaddingTop(4)
                            .AlignRight()
                            .Text(prescription.Status.ToString())
                            .FontSize(9)
                            .Bold()
                            .FontColor(MutedColor);
                    });
            });


        }



        private void ComposeContent(IContainer container)
        {
            container
                .AlignTop()
                .Border(1)
                .BorderColor(BorderColor)
                .Padding(10)
                .Column(column =>
                {
                    column.Item()
                        .Element(ComposePatientInformation);

                    column.Item()
                        .PaddingTop(24)
                        .Element(ComposePrescriptionInformation);

                    column.Item()
                        .PaddingTop(24)
                        .Element(ComposeMedications);

                    if (!string.IsNullOrWhiteSpace(prescription.Notes))
                    {
                        column.Item()
                            .PaddingTop(24)
                            .Element(ComposeNotes);
                    }
                });
        }
        private void ComposePatientInformation(IContainer container)
        {
            container.Column(column =>
            {
                column.Item()
                    .Text("PATIENT INFORMATION")
                    .FontSize(11)
                    .Bold()
                    .FontColor(PrimaryColor);

                column.Item()
                    .PaddingTop(6)
                    .BorderBottom(1)
                    .BorderColor(BorderColor);

                column.Item()
                    .PaddingTop(12)
                    .Row(row =>
                    {
                        row.RelativeItem(1.5f)
                            .Element(field =>
                                ComposeField(
                                    field,
                                    "Patient Name",
                                    prescription.PatientName));

                        row.RelativeItem()
                            .Element(field =>
                                ComposeField(
                                    field,
                                    "Document ID",
                                    prescription.PatientDocumentId));
                    });

                column.Item()
                    .PaddingTop(12)
                    .Element(field =>
                        ComposeField(
                            field,
                            "Age",
                            prescription.PatientAge.ToString()));
            });
        }

        private void ComposePrescriptionInformation(IContainer container)
        {
            container.Column(column =>
            {
                column.Item()
                    .Text("PRESCRIPTION INFORMATION")
                    .FontSize(11)
                    .Bold()
                    .FontColor(PrimaryColor);

                column.Item()
                    .PaddingTop(6)
                    .BorderBottom(1)
                    .BorderColor(BorderColor);

                column.Item()
                    .PaddingTop(12)
                    .Row(row =>
                    {
                        row.RelativeItem(1.5f)
                            .Element(field =>
                                ComposeField(
                                    field,
                                    "Doctor",
                                    prescription.DoctorName));

                        row.RelativeItem()
                            .Element(field =>
                                ComposeField(
                                    field,
                                    "Created Date",
                                    prescription.CreatedAtUtc.ToString("dd MMM yyyy")));
                    });

                column.Item()
                    .PaddingTop(12)
                    .Row(row =>
                    {
                        row.RelativeItem(1.5f)
                            .Element(field =>
                                ComposeField(
                                    field,
                                    "Valid From",
                                    prescription.ValidFrom.ToString("dd MMM yyyy")));

                        row.RelativeItem()
                            .Element(field =>
                                ComposeField(
                                    field,
                                    "Valid To",
                                    prescription.ValidTo.ToString("dd MMM yyyy")));
                    });
            });
        }
        private void ComposeMedications(IContainer container)
        {
            container.Column(column =>
            {
                column.Item()
                    .Text("MEDICATIONS")
                    .FontSize(11)
                    .Bold()
                    .FontColor(PrimaryColor);

                column.Item()
                    .PaddingTop(8)
                    .Table(table =>
                    {
                        table.ColumnsDefinition(columns =>
                        {
                            columns.RelativeColumn(1.80f); // Code
                            columns.RelativeColumn(2.40f); // Medicine
                            columns.RelativeColumn(1.20f); // Strength
                            columns.RelativeColumn(0.90f); // Form
                            columns.RelativeColumn(1.10f); // Quantity Prescribed
                            columns.RelativeColumn(0.95f); // Stock Unit
                            columns.RelativeColumn(0.95f); // Max Fill Count
                        });

                        table.Header(header =>
                        {
                            header.Cell()
                                .Element(HeaderCell)
                                .Text("Code");

                            header.Cell()
                                .Element(HeaderCell)
                                .Text("Medicine");

                            header.Cell()
                                .Element(HeaderCell)
                                .Text("Strength");

                            header.Cell()
                                .Element(HeaderCell)
                                .Text("Form");

                            header.Cell()
                                .Element(HeaderCell)
                                .AlignCenter()
                                .Text("Quantity Prescribed");

                            header.Cell()
                                .Element(HeaderCell)
                                .AlignCenter()
                                .Text("Stock Unit");

                            header.Cell()
                                .Element(HeaderCell)
                                .AlignCenter()
                                .Text("Max Fill Count");
                        });

                        foreach (var item in prescription.Items)
                        {
                            table.Cell()
                                .Element(DataCell)
                                .Text(item.MedicineCode);

                            table.Cell()
                                .Element(DataCell)
                                .Text(item.MedicineName);

                            table.Cell()
                                .Element(DataCell)
                                .Text(item.Strength);

                            table.Cell()
                                .Element(DataCell)
                                .Text(item.Form.ToString());

                            table.Cell()
                                .Element(DataCell)
                                .AlignCenter()
                                .Text(item.QuantityPrescribed.ToString());

                            table.Cell()
                                .Element(DataCell)
                                .AlignCenter()
                                .Text(item.StockUnit.ToString());

                            table.Cell()
                                .Element(DataCell)
                                .AlignCenter()
                                .Text(item.MaxFillCount.ToString());

                            table.Cell()
                                .ColumnSpan(7)
                                .Element(DosageCell)
                                .Text(text =>
                                {
                                    text.Span("Dosage Instructions: ")
                                        .Bold()
                                        .FontColor(MutedColor);

                                    text.Span(
                                            string.IsNullOrWhiteSpace(item.DosageInstructions)
                                                ? "Not specified"
                                                : item.DosageInstructions)
                                        .FontColor(TextColor);
                                });
                        }
                    });
            });
        }
        private void ComposeNotes(IContainer container)
        {
            container.Column(column =>
            {
                column.Item()
                    .Text("NOTES")
                    .FontSize(11)
                    .Bold()
                    .FontColor(PrimaryColor);

                column.Item()
                    .PaddingTop(6)
                    .BorderBottom(1)
                    .BorderColor(BorderColor);

                column.Item()
                    .PaddingTop(10)
                    .Text(prescription.Notes!)
                    .FontSize(9)
                    .FontColor(TextColor);
            });
        }
        private void ComposeFooter(IContainer container)
        {
            container
                .PaddingTop(8)
                .Row(row =>
                {
                    row.RelativeItem()
                        .AlignLeft()
                        .Text($"Generated: {generatedAt:dd MMM yyyy}")
                        .FontSize(8)
                        .FontColor(MutedColor);

                    row.RelativeItem()
                        .AlignCenter()
                        .Text(text =>
                        {
                            text.DefaultTextStyle(style =>
                                style.FontSize(8)
                                    .FontColor(MutedColor));

                            text.Span("Page ");
                            text.CurrentPageNumber();
                            text.Span(" of ");
                            text.TotalPages();
                        });

                    // Keeps page number centered.
                    row.RelativeItem();
                });
        }
        private void ComposeField(
            IContainer container,
            string label,
            string value)
        {
            container.Column(column =>
            {
                column.Item()
                    .Text(label)
                    .FontSize(8)
                    .FontColor(MutedColor);

                column.Item()
                    .PaddingTop(3)
                    .Text(value)
                    .FontSize(10)
                    .Medium()
                    .FontColor(TextColor);
            });
        }


        private IContainer HeaderCell(IContainer container)
        {
            return container
                .Background(PrimaryColor)
                .BorderRight(1)
                .BorderColor(BorderColor)
                .PaddingVertical(9)
                .PaddingHorizontal(5)
                .DefaultTextStyle(style =>
                    style.FontSize(9)
                        .Bold()
                        .FontColor(Colors.White));
        }

        private IContainer DataCell(IContainer container)
        {
            return container
                .BorderRight(1)
                .BorderBottom(1)
                .BorderColor(BorderColor)
                .PaddingVertical(9)
                .PaddingHorizontal(5)
                .DefaultTextStyle(style =>
                    style.FontSize(9)
                        .FontColor(TextColor));
        }
        private IContainer DosageCell(IContainer container)
        {
            return container
                .Background(SurfaceColor)
                .BorderBottom(1)
                .BorderColor(BorderColor)
                .PaddingVertical(7)
                .PaddingHorizontal(5)
                .DefaultTextStyle(style =>
                    style.FontSize(8.5f));
        }
    }
}
