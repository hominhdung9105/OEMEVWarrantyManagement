using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OEMEVWarrantyManagement.Domain.Entities;


namespace OEMEVWarrantyManagement.Infrastructure.Persistence.EntityConfigurations
{
    public class PartOrderItemConfiguration : IEntityTypeConfiguration<PartOrderItem>
    {
        public void Configure(EntityTypeBuilder<PartOrderItem> builder)
        {
            builder.ToTable("PartOrderItems");
            builder.HasKey(poi => poi.OrderItemId);
            builder.Property(poi => poi.OrderItemId).ValueGeneratedOnAdd();
            builder.Property(poi => poi.OrderId);
            builder.Property(poi => poi.Model).IsRequired();
            builder.Property(poi => poi.Quantity);
            builder.Property(poi => poi.Remarks);

            builder.HasOne(poi => poi.PartOrder)
                   .WithMany(po => po.PartOrderItems)
                   .HasForeignKey(poi => poi.OrderId);
        }
    }
}
