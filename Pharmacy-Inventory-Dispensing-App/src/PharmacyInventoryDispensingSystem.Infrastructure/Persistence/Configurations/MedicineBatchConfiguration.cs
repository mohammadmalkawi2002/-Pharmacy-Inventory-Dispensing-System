using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PharmacyInventoryDispensingSystem.Domain.Entities.Batches;

namespace PharmacyInventoryDispensingSystem.Infrastructure.Persistence.Configurations;

public class MedicineBatchConfiguration : IEntityTypeConfiguration<MedicineBatch>
{
    public void Configure(EntityTypeBuilder<MedicineBatch> builder)
    {
        builder.ToTable("MedicineBatches", table =>
        {
            table.HasCheckConstraint(
                "CK_MedicineBatch_QuantityInStock_NonNegative",
                "[QuantityInStock] >= 0");
        });

        builder.ConfigureSoftDeletable();

        builder.Property(b => b.MedicineId).IsRequired();

        builder.Property(b => b.BatchNumber)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(b => b.ExpiryDate).IsRequired();

        builder.Property(b => b.QuantityInStock)
            .IsRequired()
            .HasDefaultValue(0);

        builder.Property(b => b.ReceivedAt).IsRequired();

        builder.HasMany(b => b.StockMovements)
            .WithOne(sm => sm.MedicineBatch)
            .HasForeignKey(sm => sm.MedicineBatchId)
            .OnDelete(DeleteBehavior.Restrict)
            .IsRequired();

        builder.Navigation(b => b.StockMovements)
            .UsePropertyAccessMode(PropertyAccessMode.Field);

        builder.HasMany(b => b.DispenseItems)
            .WithOne(di => di.MedicineBatch)
            .HasForeignKey(di => di.MedicineBatchId)
            .OnDelete(DeleteBehavior.Restrict)
            .IsRequired();

        builder.Navigation(b => b.DispenseItems)
            .UsePropertyAccessMode(PropertyAccessMode.Field);

        builder.HasIndex(b => new { b.MedicineId, b.BatchNumber }).IsUnique();
        builder.HasIndex(b => new { b.MedicineId, b.ExpiryDate, b.ReceivedAt });
    }
}
