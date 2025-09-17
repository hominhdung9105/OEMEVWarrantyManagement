using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace OEMEVWarrantyManagement.Database.Models.Configurations
{
    public class CarInfoConfiguration : IEntityTypeConfiguration<CarInfo>
    {
        public void Configure(EntityTypeBuilder<CarInfo> builder)
        {
            builder.ToTable("CarInfo");
            builder.HasKey(ci => ci.VIN);
            builder.Property(ci => ci.VIN).IsRequired().HasMaxLength(17);
            builder.Property(ci => ci.CustomerId).IsRequired().HasMaxLength(50);
            builder.Property(ci => ci.ModelId).IsRequired().HasMaxLength(50);

            builder.HasOne(ci => ci.CarModel)
                   .WithMany(cm => cm.CarInfos)
                   .HasForeignKey(ci => ci.ModelId);

            builder.HasOne(ci => ci.Customer)
                   .WithMany(c => c.CarInfos)
                   .HasForeignKey(ci => ci.CustomerId);
        }
    }
}
