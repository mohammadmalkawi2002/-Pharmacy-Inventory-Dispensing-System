using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PharmacyInventoryDispensingSystem.Domain.Entities.Prescriptions;
using PharmacyInventoryDispensingSystem.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace PharmacyInventoryDispensingSystem.Infrastructure.Persistence.Configurations
{
    public class PrescriptionConfiguration : IEntityTypeConfiguration<Prescription>
    {
        public void Configure(EntityTypeBuilder<Prescription> builder)
        {
            builder.HasKey(p=>p.Id);

            builder.Property(p => p.PrescriptionNumber)
                .IsRequired()
                .HasMaxLength(50);

            

            builder.Property(p=>p.PatientName)
                .IsRequired()
                .HasMaxLength (100);

            builder.Property(p => p.PatientPhone)
                .IsRequired(false)
                .HasMaxLength(20);

            builder.Property(p=>p.Notes)
                .IsRequired(false)
                .HasMaxLength(300);

            builder.Property(p => p.ValidFrom)
                .IsRequired();

            builder.Property(p => p.ValidTo)
                .IsRequired();


            //Relationships:
            builder.HasMany(p => p.Items)
                .WithOne(i => i.Prescription)
                .HasForeignKey(i => i.PrescriptionId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasMany(p=>p.Dispenses)
                .WithOne(di => di.Prescription)
                .HasForeignKey(di => di.PrescriptionId)
                .OnDelete(DeleteBehavior.Restrict);

            //index:
            builder.HasIndex(p=>p.PrescriptionNumber).IsUnique();
           // builder.HasIndex(p => new { p.Status, p.ValidTo, p.PatientPhone });

            //Qyery filter: 
            builder.HasQueryFilter(p => !p.IsDeleted);








        }
    }
}
