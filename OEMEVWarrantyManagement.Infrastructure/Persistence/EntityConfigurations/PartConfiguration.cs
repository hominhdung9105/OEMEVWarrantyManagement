using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OEMEVWarrantyManagement.Domain.Entities;


namespace OEMEVWarrantyManagement.Infrastructure.Persistence.EntityConfigurations
{
    public class PartConfiguration : IEntityTypeConfiguration<Part>
    {
        public void Configure(EntityTypeBuilder<Part> builder)
        {
            builder.ToTable("Parts");
            builder.HasKey(p => p.PartId);

            // Unique model per organization
            builder.HasIndex(p => new { p.OrgId, p.Model }).IsUnique();

            builder.Property(p => p.PartId).ValueGeneratedOnAdd();
            builder.Property(p => p.Model).IsRequired();
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
