using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PharmacyInventoryDispensingSystem.Domain.Entities.Prescriptions;

namespace PharmacyInventoryDispensingSystem.Infrastructure.Persistence.Configurations;

public class PrescriptionItemConfiguration : IEntityTypeConfiguration<PrescriptionItem>
{
    public void Configure(EntityTypeBuilder<PrescriptionItem> builder)
    {
        builder.ToTable("PrescriptionItems");
        builder.ConfigureAuditable();

        builder.Property(pi => pi.PrescriptionId).IsRequired();
        builder.Property(pi => pi.MedicineId).IsRequired();

        builder.Property(pi => pi.QuantityPrescribed).IsRequired();
        builder.Property(pi => pi.QuantityDispensed).IsRequired();

        builder.Property(pi => pi.DosageInstructions)
            .IsRequired(false)
            .HasMaxLength(250);

        builder.Ignore(pi => pi.RemainingQuantity);

        builder.HasMany(pi => pi.DispenseItems)
            .WithOne(di => di.PrescriptionItem)
            .HasForeignKey(di => di.PrescriptionItemId)
            .OnDelete(DeleteBehavior.Restrict)
            .IsRequired();

        builder.HasIndex(pi => pi.PrescriptionId);
        builder.HasIndex(pi => pi.MedicineId);
    }
}
