using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PharmacyInventoryDispensingSystem.Domain.Entities.Dispenses;

namespace PharmacyInventoryDispensingSystem.Infrastructure.Persistence.Configurations;

public class DispenseConfiguration : IEntityTypeConfiguration<Dispense>
{
    public void Configure(EntityTypeBuilder<Dispense> builder)
    {
        builder.ToTable("Dispenses");
        builder.ConfigureAuditable();

        builder.Property(d => d.PrescriptionId).IsRequired();

        builder.Property(d => d.PharmacistId)
            .IsRequired()
            .HasMaxLength(450);

        builder.Property(d => d.DispensedAt).IsRequired();

        builder.Property(d => d.Notes)
            .IsRequired(false)
            .HasMaxLength(300);

        builder.HasMany(d => d.Items)
            .WithOne(di => di.Dispense)
            .HasForeignKey(di => di.DispenseId)
            .OnDelete(DeleteBehavior.Restrict)
            .IsRequired();

        builder.HasIndex(d => new { d.PrescriptionId, d.DispensedAt });
    }
}
