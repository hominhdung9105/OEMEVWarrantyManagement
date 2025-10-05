using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OEMEVWarrantyManagement.Domain.Entities;


namespace OEMEVWarrantyManagement.Infrastructure.Persistence.EntityConfigurations
{
    public class VehiclePartConfiguration : IEntityTypeConfiguration<VehiclePart>
    {
        public void Configure(EntityTypeBuilder<VehiclePart> builder)
        {
            builder.ToTable("VehicleParts");
            builder.HasKey(vp => vp.VehiclePartId);
            builder.Property(vp => vp.VehiclePartId).ValueGeneratedOnAdd();
            builder.Property(vp => vp.Vin);
            builder.Property(vp => vp.PartId);
            builder.Property(vp => vp.SerialNumber);
            builder.Property(vp => vp.InstalledDate);
            builder.Property(vp => vp.Status);

            builder.HasOne(vp => vp.Vehicle)
                   .WithMany(v => v.VehicleParts)
                   .HasForeignKey(vp => vp.Vin);

            builder.HasOne(vp => vp.Part)
                   .WithMany(p => p.VehicleParts)
                   .HasForeignKey(vp => vp.PartId);
        }
    }
}
