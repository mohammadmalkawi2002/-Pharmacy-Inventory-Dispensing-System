using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PharmacyInventoryDispensingSystem.Domain.Entities.Prescriptions;

namespace PharmacyInventoryDispensingSystem.Infrastructure.Persistence.Configurations;

public class PrescriptionConfiguration : IEntityTypeConfiguration<Prescription>
{
    public void Configure(EntityTypeBuilder<Prescription> builder)
    {
        builder.ToTable("Prescriptions");
        builder.ConfigureSoftDeletable();

        builder.Property(p => p.PrescriptionNumber)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(p => p.PatientName)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(p => p.PatientPhone)
            .IsRequired(false)
            .HasMaxLength(20);

        builder.Property(p => p.DoctorId)
            .IsRequired()
            .HasMaxLength(450);

        builder.Property(p => p.ValidFrom).IsRequired();
        builder.Property(p => p.ValidTo).IsRequired();
        builder.Property(p => p.MaxRefills).IsRequired();
        builder.Property(p => p.RefillsUsed).IsRequired();

        builder.Property(p => p.Status)
            .IsRequired()
            .HasConversion<int>();

        builder.Property(p => p.Notes)
            .IsRequired(false)
            .HasMaxLength(300);

        builder.HasMany(p => p.Items)
            .WithOne(i => i.Prescription)
            .HasForeignKey(i => i.PrescriptionId)
            .OnDelete(DeleteBehavior.Restrict)
            .IsRequired();

        builder.Navigation(p => p.Items)
            .UsePropertyAccessMode(PropertyAccessMode.Field);

        builder.HasMany(p => p.Dispenses)
            .WithOne(d => d.Prescription)
            .HasForeignKey(d => d.PrescriptionId)
            .OnDelete(DeleteBehavior.Restrict)
            .IsRequired();

        builder.Navigation(p => p.Dispenses)
            .UsePropertyAccessMode(PropertyAccessMode.Field);

        builder.HasIndex(p => p.PrescriptionNumber).IsUnique();
        builder.HasIndex(p => new { p.Status, p.ValidTo });
    }
}
