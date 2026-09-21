using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PharmacyInventoryDispensingSystem.Domain.Entities.FileSystem;
using System;
using System.Collections.Generic;
using System.Text;

namespace PharmacyInventoryDispensingSystem.Infrastructure.Persistence.Configurations
{
    public sealed class FileImageConfiguration : IEntityTypeConfiguration<FileImage>
    {
        public void Configure(EntityTypeBuilder<FileImage> builder)
        {
            builder.ToTable("FileImages");

            builder.Property(x => x.OriginalFileName)
             .IsRequired()
            .HasMaxLength(255);

            builder.Property(x => x.StoredFileName)
                .IsRequired()
                .HasMaxLength(255);

            builder.Property(x => x.ContentType)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(x => x.Extension)
                .IsRequired()
                .HasMaxLength(50);

            builder.Property(x => x.FileSize)
                .IsRequired();

            builder.HasIndex(x => x.StoredFileName)
                .IsUnique();

        }
    }
}
