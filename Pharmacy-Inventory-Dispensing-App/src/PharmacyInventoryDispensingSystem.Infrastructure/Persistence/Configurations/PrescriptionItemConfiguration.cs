using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PharmacyInventoryDispensingSystem.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace PharmacyInventoryDispensingSystem.Infrastructure.Persistence.Configurations
{
    public class PrescriptionItemConfiguration : IEntityTypeConfiguration<PrescriptionItem>
    {
        public void Configure(EntityTypeBuilder<PrescriptionItem> builder)
        {
            builder.HasKey(pi => pi.Id);
            builder.Property(pi => pi.QuantityPrescribed)
                .IsRequired();
            builder.Property(pi => pi.QuantityDispensed)
                .IsRequired();

            builder.HasOne(pi=>pi.Prescription)
                .WithMany(p=>p.Items)
                .HasForeignKey(pi=>pi.PrescriptionId);

            builder.Property(pi => pi.DosageInstructions).IsRequired().HasMaxLength(250);

               


                
        }
    }
}
