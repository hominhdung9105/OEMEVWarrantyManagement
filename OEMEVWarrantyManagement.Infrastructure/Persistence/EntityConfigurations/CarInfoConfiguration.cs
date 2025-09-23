using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OEMEVWarrantyManagement.Domain.Entities;

namespace OEMEVWarrantyManagement.Infrastructure.Persistence.EntityConfigurations
{
    public class CarInfoConfiguration : IEntityTypeConfiguration<CarInfo>
    {
        public void Configure(EntityTypeBuilder<CarInfo> builder)
        {
            builder.ToTable("CarInfo");
            builder.HasKey(ci => ci.VIN);
            builder.Property(ci => ci.VIN).IsRequired().HasMaxLength(17);
            builder.Property(ci => ci.CustomerId).IsRequired();
            builder.Property(ci => ci.ModelId).IsRequired();

            builder.HasOne(ci => ci.CarModel)
                   .WithMany(cm => cm.CarInfos)
                   .HasForeignKey(ci => ci.ModelId);

            builder.HasOne(ci => ci.Customer)
                   .WithMany(c => c.CarInfos)
                   .HasForeignKey(ci => ci.CustomerId);
        }
    }
}
