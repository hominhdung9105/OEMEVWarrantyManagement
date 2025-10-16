using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OEMEVWarrantyManagement.Domain.Entities;


namespace OEMEVWarrantyManagement.Infrastructure.Persistence.EntityConfigurations
{
    public class VehicleConfiguration : IEntityTypeConfiguration<Vehicle>
    {
        public void Configure(EntityTypeBuilder<Vehicle> builder)
        {
            builder.ToTable("Vehicles");
            builder.HasKey(v => v.Vin);

            builder.Property(v => v.Vin).IsRequired();
            builder.Property(v => v.Model).IsRequired();
            builder.Property(v => v.Year);
            builder.Property(v => v.CustomerId);

            builder.HasOne(v => v.Customer)
                   .WithMany(c => c.Vehicles)
                   .HasForeignKey(v => v.CustomerId)
                   .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
