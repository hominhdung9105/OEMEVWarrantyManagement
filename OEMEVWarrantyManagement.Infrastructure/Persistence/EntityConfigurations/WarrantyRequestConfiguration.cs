using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OEMEVWarrantyManagement.Domain.Entities;

namespace OEMEVWarrantyManagement.Infrastructure.Persistence.EntityConfigurations
{
    public class WarrantyRequestConfiguration : IEntityTypeConfiguration<WarrantyRequest>
    {
        public void Configure(EntityTypeBuilder<WarrantyRequest> builder)
        {
            builder.ToTable("WarrantyRequests");
            builder.HasKey(wr => wr.Id);
            builder.Property(wr => wr.Id).ValueGeneratedOnAdd();
            builder.Property(wr => wr.RequestDate);
            builder.Property(wr => wr.ResponseDate);
            builder.Property(wr => wr.Status)
                   .IsRequired()
                   .HasMaxLength(50);
            builder.Property(wr => wr.VIN)
                   .IsRequired();
            builder.Property(wr => wr.SCStaffId)
                   .IsRequired();
            builder.Property(wr => wr.EVMStaffId);

            builder.HasOne(wr => wr.CarConditionCurrent)
                   .WithOne(ccc => ccc.WarrantyRequest)
                   .HasForeignKey<CarConditionCurrent>(ccc => ccc.WarrantyRequestId)
                   .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(wr => wr.EVMStaff)
                   .WithMany(e => e.WarrantyRequestsAsEVMStaff)
                   .HasForeignKey(wr => wr.EVMStaffId)
                   .OnDelete(DeleteBehavior.Restrict);
                   //.IsRequired(false);

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
