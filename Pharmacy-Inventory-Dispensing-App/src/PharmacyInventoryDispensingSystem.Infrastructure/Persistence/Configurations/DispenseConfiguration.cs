using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PharmacyInventoryDispensingSystem.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace PharmacyInventoryDispensingSystem.Infrastructure.Persistence.Configurations
{
    public class DispenseConfiguration : IEntityTypeConfiguration<Dispense>
    {
        public void Configure(EntityTypeBuilder<Dispense> builder)
        {
            builder.HasKey(d => d.Id);
          //  builder.Property(d => d.PharmacistId).IsRequired().HasMaxLength(100);

            builder.Property(d=>d.Notes).IsRequired(false).HasMaxLength(300);



           builder.HasMany(d=>d.Items)
                .WithOne(di=>di.Dispense)
                .HasForeignKey(di => di.DispenseId)
                .OnDelete(DeleteBehavior.Restrict);




        }
    }
}
