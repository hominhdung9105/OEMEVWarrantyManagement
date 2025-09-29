using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OEMEVWarrantyManagement.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OEMEVWarrantyManagement.Infrastructure.Persistence.EntityConfigurations
{
    public class PartConfiguration : IEntityTypeConfiguration<Part>
    {
        public void Configure(EntityTypeBuilder<Part> builder)
        {
            builder.ToTable("Parts");
            builder.HasKey(p => p.PartId);
            builder.HasIndex(p => p.PartNumber).IsUnique();

            builder.Property(p => p.PartId).ValueGeneratedOnAdd();
            builder.Property(p => p.PartNumber).IsRequired();
            builder.Property(p => p.Name).IsRequired();
            builder.Property(p => p.Category);
            builder.Property(p => p.StockQuantity);
            builder.Property(p => p.OrgId);

            builder.HasOne(p => p.Organization)
                   .WithMany(o => o.Parts)
                   .HasForeignKey(p => p.OrgId)
                   .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
