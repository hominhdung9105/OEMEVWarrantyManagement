using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace OEMEVWarrantyManagement.Database.Models.Configurations
{
    public class PartsRequestPartConfiguration : IEntityTypeConfiguration<PartsRequestPart>
    {
        public void Configure(EntityTypeBuilder<PartsRequestPart> builder)
        {
            builder.ToTable("PartsRequestParts");
            builder.HasKey(prp => new { prp.RequestPartId, prp.PartsId });
            // Relationship with RequestPart 1-N
            builder.HasOne(prp => prp.RequestPart)
                   .WithMany(rp => rp.PartsRequestParts)
                   .HasForeignKey(prp => prp.RequestPartId)
                   .OnDelete(DeleteBehavior.Cascade);
            // Relationship with Parts 1-N
            builder.HasOne(prp => prp.Parts)
                   .WithMany(p => p.PartsRequestParts)
                   .HasForeignKey(prp => prp.PartsId)
                   .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
