using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace OEMEVWarrantyManagement.Database.Models.Configurations
{
    public class WarrantyRequestConfiguration : IEntityTypeConfiguration<WarrantyRequest>
    {
        public void Configure(EntityTypeBuilder<WarrantyRequest> builder)
        {
            builder.ToTable("WarrantyRequests");
            builder.HasKey(wr => wr.Id);
            builder.Property(wr => wr.RequestDate)
                   .IsRequired();
            builder.Property(wr => wr.ResponseDate);
            builder.Property(wr => wr.status)
                   .IsRequired()
                   .HasMaxLength(50);
            builder.Property(wr => wr.VIN)
                   .IsRequired();
            builder.Property(wr => wr.SCStaffId)
                   .IsRequired();
            builder.Property(wr => wr.EVMStaffId)
                   .IsRequired();
            builder.Property(wr => wr.CarConditionCurrentId)
                   .IsRequired();

            builder.HasOne(wr => wr.CarConditionCurrent)
                   .WithMany(ccc => ccc.WarrantyRequests)
                   .HasForeignKey(wr => wr.CarConditionCurrentId);

            builder.HasOne(wr => wr.EVMStaff)
                   .WithMany(e => e.WarrantyRequestsAsEVMStaff)
                   .HasForeignKey(wr => wr.EVMStaffId)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(wr => wr.SCStaff)
                   .WithMany(e => e.WarrantyRequestsAsSCStaff)
                   .HasForeignKey(wr => wr.SCStaffId)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(wr => wr.CarInfo)
                   .WithMany(ci => ci.WarrantyRequests)
                   .HasForeignKey(wr => wr.VIN)
                   .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
