using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PharmacyInventoryDispensingSystem.Domain.Entities.Dispenses;
using System;
using System.Collections.Generic;
using System.Text;

namespace PharmacyInventoryDispensingSystem.Infrastructure.Persistence.Configurations
{
    public class DispenseItemConfiguration : IEntityTypeConfiguration<DispenseItem>
    {
        public void Configure(EntityTypeBuilder<DispenseItem> builder)
        {
            builder.HasKey(di => di.Id);


            builder.HasOne(di => di.PrescriptionItem)
                .WithMany(pi => pi.DispenseItems)
                .HasForeignKey(di => di.PrescriptionItemId)
                .OnDelete(DeleteBehavior.Restrict);

          
        }
    }
}
