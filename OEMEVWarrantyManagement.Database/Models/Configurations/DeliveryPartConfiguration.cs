using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace OEMEVWarrantyManagement.Database.Models.Configurations
{
    public class DeliveryPartConfiguration : IEntityTypeConfiguration<DeliveryPart>
    {
        public void Configure(EntityTypeBuilder<DeliveryPart> builder)
        {
            builder.ToTable("DeliveryParts");
            builder.HasKey(dp => dp.Id);
            builder.Property(dp => dp.StaffSend)
                   .IsRequired();
            builder.Property(dp => dp.StaffReceive)
                   .IsRequired();
            builder.Property(dp => dp.Status)
                   .IsRequired()
                   .HasMaxLength(50);
            builder.Property(dp => dp.LocationId)
                   .IsRequired()
                   .HasMaxLength(100);
            builder.Property(dp => dp.DateSend)
                   .IsRequired();
            builder.Property(dp => dp.DateReceive)
                   .IsRequired();

            //relationship StaffSend - Employee
            builder.HasOne(dp => dp.StaffSendEmployee)
                   .WithMany(e => e.DeliveryPartsSend)
                   .HasForeignKey(dp => dp.StaffSend)
                   .OnDelete(DeleteBehavior.Restrict);
            //relationship StaffReceive - Employee
            builder.HasOne(dp => dp.StaffReceiveEmployee)
                   .WithMany(e => e.DeliveryPartsReceive)
                   .HasForeignKey(dp => dp.StaffReceive)
                   .OnDelete(DeleteBehavior.Restrict);
            builder.HasOne(dp => dp.WorkPlaces)
                   .WithMany(wp => wp.DeliveryParts) // Trỏ đến collection trong WorkPlace
                   .HasForeignKey(dp => dp.WorkPlaceId)
                   .IsRequired();
        }
    }

}
