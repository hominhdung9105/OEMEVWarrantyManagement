using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace OEMEVWarrantyManagement.Database.Models.Configurations
{
    public class PartsDeliveryPartConfiguration : IEntityTypeConfiguration<PartsDeliveryPart>
    {
        public void Configure(EntityTypeBuilder<PartsDeliveryPart> builder)
        {
            builder.ToTable("PartsDeliveryParts");
            builder.HasKey(pdp => new { pdp.DeliveryPartId, pdp.PartsId });

            // Relationship with DeliveryPart 1-N
            builder.HasOne(pdp => pdp.DeliveryPart)
                   .WithMany(dp => dp.PartsDeliveryParts)
                   .HasForeignKey(pdp => pdp.DeliveryPartId)
                   .OnDelete(DeleteBehavior.Cascade);
            // Relationship with Parts 1-N
            builder.HasOne(pdp => pdp.Parts)
                   .WithMany(p => p.PartsDeliveryParts)
                   .HasForeignKey(pdp => pdp.PartsId)
                   .OnDelete(DeleteBehavior.Cascade);

        }
    }


}
