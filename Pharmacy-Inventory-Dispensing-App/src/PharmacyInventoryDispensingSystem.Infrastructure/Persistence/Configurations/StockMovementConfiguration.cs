using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PharmacyInventoryDispensingSystem.Domain.Entities.StockMovements;
using System;
using System.Collections.Generic;
using System.Text;

namespace PharmacyInventoryDispensingSystem.Infrastructure.Persistence.Configurations
{
    public class StockMovementConfiguration : IEntityTypeConfiguration<StockMovement>
    {
        public void Configure(EntityTypeBuilder<StockMovement> builder)
        {
            builder.ToTable("StockMovements");

            builder.HasKey(sm => sm.Id);

            builder.Property(sm => sm.MovementType)
                .IsRequired()
                .HasConversion<int>();

            builder.Property(sm=>sm.Reason)
                .IsRequired(false)
                .HasMaxLength(200);

            builder.Property(sm=>sm.MedicineBatchId)
                .IsRequired();

            builder.Property(sm => sm.QuantityChange)
                .IsRequired();

          
            


        }
    }
}
