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
                .HasMaxLength(9);


            //index:
            builder.HasIndex(p => p.PrescriptionNumber).IsUnique();

            builder.Property(p => p.PatientId)
            .IsRequired();

            builder.Property(p => p.DoctorId)
                .IsRequired();


            builder.Property(p=>p.Notes)
                .IsRequired(false)
                .HasMaxLength(500);

            builder.Property(p => p.ValidFrom)
                .IsRequired();

            builder.Property(p => p.ValidTo)
                .IsRequired();

            builder.Property(p => p.Status)
                .IsRequired()
                .HasConversion<int>();


            //Relationships:
            builder.HasMany(p => p.Items)
                .WithOne(i => i.Prescription)
                .HasForeignKey(i => i.PrescriptionId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(p => p.Patient)
            .WithMany(patient => patient.Prescriptions)
            .HasForeignKey(p => p.PatientId)
            .OnDelete(DeleteBehavior.Restrict);


            builder.HasMany(p=>p.Dispenses)
                .WithOne(di => di.Prescription)
                .HasForeignKey(di => di.PrescriptionId)
                .OnDelete(DeleteBehavior.Restrict);

            

            







        }
    }
}
