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
            builder.ToTable("PrescriptionItems");
            
            builder.HasKey(pi => pi.Id);

            builder.Property(pi => pi.PrescriptionId)
                .IsRequired();

            builder.Property(pi => pi.MedicineId)
                .IsRequired();

            builder.Property(pi => pi.QuantityPrescribed)
                .IsRequired();

            builder.Property(pi => pi.MaxFillCount)
                .IsRequired();

            builder.Property(pi => pi.FillUsedCount)
                .IsRequired();

            builder.Property(pi => pi.DosageInstructions)
                .HasMaxLength(500);

            // Relationships

            builder.HasOne(pi => pi.Medicine)
                .WithMany(medicine => medicine.PrescriptionItems)
                .HasForeignKey(pi => pi.MedicineId)
                .OnDelete(DeleteBehavior.Restrict);

            // A medicine can appear only once within the same prescription.
            builder.HasIndex(pi => new
            {
                pi.PrescriptionId,
                pi.MedicineId
            })
            .IsUnique();











        }
    }
}
