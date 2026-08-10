using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PharmacyInventoryDispensingSystem.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace PharmacyInventoryDispensingSystem.Infrastructure.Persistence.Configurations
{
    public class MedicineBatchConfiguration : IEntityTypeConfiguration<MedicineBatch>
    {
        public void Configure(EntityTypeBuilder<MedicineBatch> builder)
        {
            builder.HasKey(b => b.Id);
            builder.ToTable("MedicineBatches", table => 
            { 
                table.HasCheckConstraint("CK_MedicineBatch_QuantityInStock_NonNegative",
                         "[QuantityInStock] >= 0");
            });
            builder.Property(b => b.MedicineId).IsRequired();
            builder.Property(b => b.BatchNumber).IsRequired().HasMaxLength(50);
            builder.Property(b => b.QuantityInStock).HasDefaultValue(0);

            // Relationships:
            builder.HasMany(b=>b.DispenseItems)
                .WithOne(di => di.MedicineBatch)
                .HasForeignKey(di => di.MedicineBatchId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasMany(b=>b.StockMovements)
                .WithOne(sm => sm.MedicineBatch)
                .HasForeignKey(sm => sm.MedicineBatchId)
                .OnDelete(DeleteBehavior.Restrict);

           



            //Query Filter:
            builder.HasQueryFilter(b => !b.IsDeleted);

            builder.HasIndex(b => new { b.MedicineId, b.BatchNumber }).IsUnique();
            

        }
    }
}
