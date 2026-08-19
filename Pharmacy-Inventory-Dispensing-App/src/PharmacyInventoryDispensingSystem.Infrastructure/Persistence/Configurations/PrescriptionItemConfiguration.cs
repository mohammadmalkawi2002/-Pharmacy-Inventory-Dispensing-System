using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PharmacyInventoryDispensingSystem.Domain.Entities.Prescriptions;
using System;
using System.Collections.Generic;
using System.Text;

namespace PharmacyInventoryDispensingSystem.Infrastructure.Persistence.Configurations
{
    public class PrescriptionItemConfiguration : IEntityTypeConfiguration<PrescriptionItem>
    {
        public void Configure(EntityTypeBuilder<PrescriptionItem> builder)
        {
            builder.ToTable("PrescriptionItems", table =>
            {
                table.HasCheckConstraint(
                    "CK_PrescriptionItems_QuantityPrescribed_Positive",
                    "[QuantityPrescribed] > 0");

                table.HasCheckConstraint(
                    "CK_PrescriptionItems_QuantityDispensed_NonNegative",
                    "[QuantityDispensed] >= 0");

                table.HasCheckConstraint(
                    "CK_PrescriptionItems_MaxRefill_NonNegative",
                    "[MaxRefill] >= 0");

                table.HasCheckConstraint(
                    "CK_PrescriptionItems_RefillUsed_NonNegative",
                    "[RefillUsed] >= 0");
            });

            builder.HasKey(pi => pi.Id);

            builder.Property(pi => pi.PrescriptionId)
                .IsRequired();

            builder.Property(pi => pi.MedicineId)
                .IsRequired();

            builder.Property(pi => pi.QuantityPrescribed)
                .IsRequired();

            builder.Property(pi => pi.QuantityDispensed)
                .IsRequired();

            builder.Property(pi => pi.MaxRefill)
                .IsRequired();

            builder.Property(pi => pi.RefillUsed)
                .IsRequired();

            builder.Property(pi => pi.DosageInstructions)
                .HasMaxLength(500);

           
            

            builder.HasOne(pi => pi.Medicine)
                .WithMany(m => m.PrescriptionItems)
                .HasForeignKey(pi => pi.MedicineId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasIndex(pi => pi.PrescriptionId);

            builder.HasIndex(pi => pi.MedicineId);











        }
    }
}
