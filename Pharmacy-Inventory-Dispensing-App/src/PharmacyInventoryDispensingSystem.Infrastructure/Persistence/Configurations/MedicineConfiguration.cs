using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PharmacyInventoryDispensingSystem.Domain.Entities.Medicines;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;

namespace PharmacyInventoryDispensingSystem.Infrastructure.Persistence.Configurations
{
    public class MedicineConfiguration : IEntityTypeConfiguration<Medicine>
    {
        public void Configure(EntityTypeBuilder<Medicine> builder)
        {

            builder.ToTable("Medicines", table => 
            {
                table.HasCheckConstraint(
                    "CK_Medicines_QuantityInStock_NonNegative",
                    "[QuantityInStock] >= 0"
                    );
                
            });

            builder.HasKey(m => m.Id);


            builder.Property(m => m.Name)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(m => m.Code)
                .IsRequired()
                .HasMaxLength(50);


            builder.Property(m => m.Strength)
                .IsRequired()
                .HasMaxLength(50);


            builder.Property(m => m.Form)
                .IsRequired()
                .HasConversion<int>();


            builder.Property(m => m.Unit)
                 .IsRequired()
                 .HasMaxLength(50);


            builder.Property(m => m.ReorderLevel)
                .IsRequired();

            builder.Property(m => m.QuantityInStock)
                .IsRequired();


            builder.Property(m => m.IsActive)
                .IsRequired()
                .HasDefaultValue(true);


            



            builder.HasMany(m => m.PrescriptionItems)
                .WithOne(pi => pi.Medicine)
                .HasForeignKey(pi => pi.MedicineId)
                .OnDelete(DeleteBehavior.Restrict);
                            

                

            //  Indexes:

            builder.HasIndex(m => m.Code)
                .IsUnique();
            // Useful for searching medicines by name
            builder.HasIndex(m => m.Name);

            


            //QueryFilter: 
            builder.HasQueryFilter(m =>! m.IsDeleted);




            
            

        }
    }
}
