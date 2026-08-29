using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PharmacyInventoryDispensingSystem.Domain.Entities.Patients;
using System;
using System.Collections.Generic;
using System.Text;

namespace PharmacyInventoryDispensingSystem.Infrastructure.Persistence.Configurations
{
    public class PatientConfiguration : IEntityTypeConfiguration<Patient>
    {
        public void Configure(EntityTypeBuilder<Patient> builder)
        {
            builder.ToTable("Patients");

            builder.HasKey(p => p.Id);

            builder.Property(p => p.DocumentId)
                .IsRequired()
                .HasMaxLength(10);

            builder.HasIndex(p => p.DocumentId)
                .IsUnique();

            builder.Property(p => p.FullName)
              .IsRequired()
              .HasMaxLength(200);

            builder.Property(p => p.DateOfBirth)
                .HasColumnType("date")
                .IsRequired();

            builder.Property(p => p.PhoneNumber)
                .IsRequired()
                .HasMaxLength(16);

            builder.HasMany(p => p.Prescriptions)
                .WithOne(p => p.Patient)
                .HasForeignKey(p => p.PatientId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasQueryFilter(P => !P.IsDeleted);

            // Active patient search/sorting by FullName.
            builder.HasIndex(patient => new
            {
                patient.FullName,
                patient.Id
            })
            .HasFilter("[IsDeleted] = 0");

            // Active patient sorting by CreatedAt.
            builder.HasIndex(patient => new
            {
                patient.CreatedAtUtc,
                patient.Id
            })
            .HasFilter("[IsDeleted] = 0");
        }
    }
}
