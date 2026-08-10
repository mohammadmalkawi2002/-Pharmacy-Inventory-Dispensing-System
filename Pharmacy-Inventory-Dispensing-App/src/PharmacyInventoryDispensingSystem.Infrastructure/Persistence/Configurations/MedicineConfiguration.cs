using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PharmacyInventoryDispensingSystem.Domain.Entities;
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
            builder.HasKey(m => m.Id);

            builder.ToTable("Medicines");

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
               .IsRequired(required: false)
                .HasMaxLength(50);

           builder.Property(m=>m.Unit)
                .IsRequired()
                .HasMaxLength(20);


            builder.Property(m => m.ReorderLevel)
                .IsRequired();


            builder.Property(m => m.IsActive)
                .HasDefaultValue(true);


            // Relationships

            // One Medicine has many Batches
            builder.HasMany(m => m.Batches)
                .WithOne(b => b.Medicine)
                .HasForeignKey(b => b.MedicineId)
                .OnDelete(DeleteBehavior.Restrict);


            // One Medicine can appear in many Prescription Items
            builder.HasMany(m => m.PrescriptionItems)
                .WithOne(pi => pi.Medicine)
                .HasForeignKey(pi => pi.MedicineId)
                .OnDelete(DeleteBehavior.Restrict);


            //  Indexes:

            builder.HasIndex(m => m.Code)
                .IsUnique();
            // Useful for searching medicines by name
            builder.HasIndex(m => m.Name);

            //another option is to create a composite index:
            // builder.HasIndex(m => new { m.Name, m.IsActive });


            //QueryFilter: 
            builder.HasQueryFilter(m =>! m.IsDeleted);




            
            

        }
    }
}
