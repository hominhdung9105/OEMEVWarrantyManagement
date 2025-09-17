using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace OEMEVWarrantyManagement.Database.Models.Configurations
{
    public class WarrantyPartsReplacementConfiguration : IEntityTypeConfiguration<WarrantyPartsReplacement>
    {
        public void Configure(EntityTypeBuilder<WarrantyPartsReplacement> builder)
        {
            builder.ToTable("WarrantyPartsInWarranty");
            builder.HasKey(wpiw => new { wpiw.WarrantyId, wpiw.PartsReplacementId });

            // relationship warranty - WarrantyPartReplacement
            builder.HasOne(wpiw => wpiw.Warranty)
                   .WithMany(w => w.WarrantyPartReplacements)
                   .HasForeignKey(wpiw => wpiw.WarrantyId)
                   .OnDelete(DeleteBehavior.Cascade);

            // relationship PartReplacement - WarrantyPartReplacement
            builder.HasOne(wpiw => wpiw.PartsReplacement)
                   .WithMany(piw => piw.WarrantyPartsInWarranties)
                   .HasForeignKey(wpiw => wpiw.PartsReplacementId)
                   .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
