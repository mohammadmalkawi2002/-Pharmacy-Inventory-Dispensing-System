using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PharmacyInventoryDispensingSystem.Domain.Entities.Medicines;

namespace PharmacyInventoryDispensingSystem.Infrastructure.Persistence.Configurations;

public class MedicineConfiguration : IEntityTypeConfiguration<Medicine>
{
    public void Configure(EntityTypeBuilder<Medicine> builder)
    {
        builder.ToTable("Medicines");
        builder.ConfigureSoftDeletable();

        builder.Property(m => m.Code)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(m => m.Name)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(m => m.Strength)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(m => m.Form)
            .IsRequired()
            .HasConversion<int>();

        builder.Property(m => m.Unit)
            .IsRequired()
            .HasMaxLength(20);

        builder.Property(m => m.ReorderLevel)
            .IsRequired();

        builder.Property(m => m.IsActive)
            .IsRequired()
            .HasDefaultValue(true);

        builder.HasMany(m => m.Batches)
            .WithOne(b => b.Medicine)
            .HasForeignKey(b => b.MedicineId)
            .OnDelete(DeleteBehavior.Restrict)
            .IsRequired();

        builder.Navigation(m => m.Batches)
            .UsePropertyAccessMode(PropertyAccessMode.Field);

        builder.HasMany(m => m.PrescriptionItems)
            .WithOne(pi => pi.Medicine)
            .HasForeignKey(pi => pi.MedicineId)
            .OnDelete(DeleteBehavior.Restrict)
            .IsRequired();

        builder.Navigation(m => m.PrescriptionItems)
            .UsePropertyAccessMode(PropertyAccessMode.Field);

        builder.HasIndex(m => m.Code).IsUnique();
        builder.HasIndex(m => m.Name);
        builder.HasIndex(m => m.IsActive);
    }
}
