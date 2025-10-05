using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OEMEVWarrantyManagement.Domain.Entities;


namespace OEMEVWarrantyManagement.Infrastructure.Persistence.EntityConfigurations
{
    public class VehicleWarrantyPolicyConfiguration : IEntityTypeConfiguration<VehicleWarrantyPolicy>
    {
        public void Configure(EntityTypeBuilder<VehicleWarrantyPolicy> builder)
        {
            builder.ToTable("VehicleWarrantyPolicies");
            builder.HasKey(vwp => vwp.VehicleWarrantyId);
            builder.Property(vwp => vwp.VehicleWarrantyId).ValueGeneratedOnAdd();
            builder.Property(vwp => vwp.Vin);
            builder.Property(vwp => vwp.PolicyId);
            builder.Property(vwp => vwp.StartDate);
            builder.Property(vwp => vwp.EndDate);
            builder.Property(vwp => vwp.Status);

            builder.HasOne(vwp => vwp.Vehicle)
                   .WithMany(v => v.VehicleWarrantyPolicies)
                   .HasForeignKey(vwp => vwp.Vin)
                   .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(vwp => vwp.WarrantyPolicy)
                   .WithMany(p => p.VehicleWarrantyPolicies)
                   .HasForeignKey(vwp => vwp.PolicyId)
                   .OnDelete(DeleteBehavior.NoAction);
        }
    }
}
