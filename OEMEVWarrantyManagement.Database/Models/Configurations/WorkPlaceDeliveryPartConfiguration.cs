using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace OEMEVWarrantyManagement.Database.Models.Configurations
{
    public class WorkPlaceDeliveryPartConfiguration : IEntityTypeConfiguration<WorkPlaceDeliveryPart>
    {
        public void Configure(EntityTypeBuilder<WorkPlaceDeliveryPart> builder)
        {
            builder.ToTable("WorkPlaceDeliveryParts");
            builder.HasKey(wdp => new { wdp.WorkPlaceId, wdp.DeliveryPartId });

            //relationship with WorkPlace
            builder.HasOne(wdp => wdp.WorkPlace)
                   .WithMany(wp => wp.WorkPlaceDeliveryParts)
                   .HasForeignKey(wdp => wdp.WorkPlaceId);

            //relationship with DeliveryPart
            builder.HasOne(wdp => wdp.DeliveryPart)
                   .WithMany(dp => dp.WorkPlaceDeliveryParts)
                   .HasForeignKey(wdp => wdp.DeliveryPartId);

        }
    }
}
