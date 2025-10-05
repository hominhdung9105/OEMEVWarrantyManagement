using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OEMEVWarrantyManagement.Domain.Entities;


namespace OEMEVWarrantyManagement.Infrastructure.Persistence.EntityConfigurations
{
    public class PartOrderConfiguration : IEntityTypeConfiguration<PartOrder>
    {
        public void Configure(EntityTypeBuilder<PartOrder> builder)
        {
            builder.ToTable("PartOrders");
            builder.HasKey(po => po.OrderId);
            builder.Property(po => po.OrderId).ValueGeneratedOnAdd();
            builder.Property(po => po.ServiceCenterId);
            builder.Property(po => po.RequestDate);
            builder.Property(po => po.ApprovedDate);
            builder.Property(po => po.ShippedDate);
            builder.Property(po => po.Status);
            builder.Property(po => po.CreatedBy);

            builder.HasOne(po => po.ServiceCenter)
                   .WithMany(o => o.PartOrders)
                   .HasForeignKey(po => po.ServiceCenterId)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(po => po.CreatedByEmployee)
                   .WithMany(e => e.CreatedPartOrders)
                   .HasForeignKey(po => po.CreatedBy)
                   .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
