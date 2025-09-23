using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OEMEVWarrantyManagement.Domain.Entities;

namespace OEMEVWarrantyManagement.Infrastructure.Persistence.EntityConfigurations
{
    public class WarrantyPartsReplacementConfiguration : IEntityTypeConfiguration<WarrantyPartsReplacement>
    {
        public void Configure(EntityTypeBuilder<WarrantyPartsReplacement> builder)
        {
            builder.ToTable("WarrantyPartsReplacement");
            builder.HasKey(wpiw => new { wpiw.WarrantyId, wpiw.PartsReplacementId });
            builder.Property(x => x.WarrantyId)
                   .HasMaxLength(100);
            builder.Property(x => x.PartsReplacementId)
                   .HasMaxLength(100);

            // relationship warranty - WarrantyPartReplacement
            builder.HasOne(wpiw => wpiw.Warranty)
                   .WithMany(w => w.WarrantyPartReplacements)
                   .HasForeignKey(wpiw => wpiw.WarrantyId)
                   .OnDelete(DeleteBehavior.Restrict);

            // relationship PartReplacement - WarrantyPartReplacement
            builder.HasOne(wpiw => wpiw.PartsReplacement)
                   .WithMany(piw => piw.WarrantyPartReplacements)
                   .HasForeignKey(wpiw => wpiw.PartsReplacementId)
                   .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
