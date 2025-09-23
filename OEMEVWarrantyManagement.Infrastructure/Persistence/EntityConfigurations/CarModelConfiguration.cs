using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OEMEVWarrantyManagement.Domain.Entities;


namespace OEMEVWarrantyManagement.Infrastructure.Persistence.EntityConfigurations
{
    public class CarModelConfiguration : IEntityTypeConfiguration<CarModel>
    {
        public void Configure(EntityTypeBuilder<CarModel> builder)
        {
            builder.HasKey(cm => cm.Id);
            builder.Property(cm => cm.Name)
                   .IsRequired();
        }
    }
}
