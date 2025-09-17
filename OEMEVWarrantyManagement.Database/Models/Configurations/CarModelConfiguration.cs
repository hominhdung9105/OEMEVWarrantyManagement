using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace OEMEVWarrantyManagement.Database.Models.Configurations
{
    public class CarModelConfiguration : IEntityTypeConfiguration<CarModel>
    {
        public void Configure(EntityTypeBuilder<CarModel> builder)
        {
            builder.HasKey(cm => cm.Id);
            builder.Property(cm => cm.Name)
                   .IsRequired()
                   .HasMaxLength(100);
        }
    }
}
