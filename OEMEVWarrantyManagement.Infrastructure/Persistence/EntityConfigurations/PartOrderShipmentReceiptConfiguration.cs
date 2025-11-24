using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OEMEVWarrantyManagement.Domain.Entities;

namespace OEMEVWarrantyManagement.Infrastructure.Persistence.EntityConfigurations
{
    public class PartOrderShipmentConfiguration : IEntityTypeConfiguration<PartOrderShipment>
    {
        public void Configure(EntityTypeBuilder<PartOrderShipment> builder)
        {
            builder.ToTable("PartOrderShipments");
            builder.HasKey(s => s.ShipmentId);
            builder.Property(s => s.ShipmentId).ValueGeneratedOnAdd();
            builder.Property(s => s.OrderId).IsRequired();
            builder.Property(s => s.Model).IsRequired().HasMaxLength(100);
            builder.Property(s => s.SerialNumber).IsRequired().HasMaxLength(100);
            builder.Property(s => s.ShippedAt).IsRequired();
            builder.Property(s => s.Status).IsRequired().HasMaxLength(50);

            builder.HasOne(s => s.PartOrder)
                   .WithMany(po => po.Shipments)
                   .HasForeignKey(s => s.OrderId)
                   .OnDelete(DeleteBehavior.Cascade);

            builder.HasIndex(s => new { s.OrderId, s.SerialNumber });
        }
    }

    public class PartOrderReceiptConfiguration : IEntityTypeConfiguration<PartOrderReceipt>
    {
        public void Configure(EntityTypeBuilder<PartOrderReceipt> builder)
        {
            builder.ToTable("PartOrderReceipts");
            builder.HasKey(r => r.ReceiptId);
            builder.Property(r => r.ReceiptId).ValueGeneratedOnAdd();
            builder.Property(r => r.OrderId).IsRequired();
            builder.Property(r => r.Model).IsRequired().HasMaxLength(100);
            builder.Property(r => r.SerialNumber).IsRequired().HasMaxLength(100);
            builder.Property(r => r.ReceivedAt);
            builder.Property(r => r.Status).IsRequired().HasMaxLength(50);
            builder.Property(r => r.Note).HasMaxLength(500);
            builder.Property(r => r.ImageUrl).HasMaxLength(500);

            builder.HasOne(r => r.PartOrder)
                   .WithMany(po => po.Receipts)
                   .HasForeignKey(r => r.OrderId)
                   .OnDelete(DeleteBehavior.Cascade);

            builder.HasIndex(r => new { r.OrderId, r.SerialNumber });
        }
    }
}
