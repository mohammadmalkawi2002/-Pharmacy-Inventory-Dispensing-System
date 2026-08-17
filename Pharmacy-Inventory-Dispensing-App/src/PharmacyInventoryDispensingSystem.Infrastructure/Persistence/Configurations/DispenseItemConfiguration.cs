using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PharmacyInventoryDispensingSystem.Domain.Entities.Dispenses;

namespace PharmacyInventoryDispensingSystem.Infrastructure.Persistence.Configurations;

public class DispenseItemConfiguration : IEntityTypeConfiguration<DispenseItem>
{
    public void Configure(EntityTypeBuilder<DispenseItem> builder)
    {
        builder.ToTable("DispenseItems");
        builder.ConfigureAuditable();

        builder.Property(di => di.DispenseId).IsRequired();
        builder.Property(di => di.PrescriptionItemId).IsRequired();
        builder.Property(di => di.MedicineBatchId).IsRequired();
        builder.Property(di => di.Quantity).IsRequired();

        builder.HasIndex(di => di.DispenseId);
        builder.HasIndex(di => di.PrescriptionItemId);
        builder.HasIndex(di => di.MedicineBatchId);
    }
}
