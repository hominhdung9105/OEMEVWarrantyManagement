using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace OEMEVWarrantyManagement.Database.Models.Configurations
{
    public class RequestPartConfiguration : IEntityTypeConfiguration<RequestPart>
    {
        public void Configure(EntityTypeBuilder<RequestPart> builder)
        {
            builder.ToTable("RequestParts");
            builder.HasKey(rp => rp.Id);
            builder.Property(rp => rp.status)
                   .IsRequired()
                   .HasMaxLength(50);
            builder.Property(rp => rp.SCStaffId)
                     .IsRequired();
            builder.Property(rp => rp.EVMStaffId)
                        .IsRequired();

            builder.HasOne(rp => rp.SCStaff)
                   .WithMany(e => e.RequestParts)
                   .HasForeignKey(rp => rp.SCStaffId)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(rp => rp.EVMStaff)
                     .WithMany()
                     .HasForeignKey(rp => rp.EVMStaffId)
                     .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
