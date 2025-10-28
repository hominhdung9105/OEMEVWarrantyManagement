using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OEMEVWarrantyManagement.Domain.Entities;

namespace OEMEVWarrantyManagement.Infrastructure.Persistence.EntityConfigurations
{
    public class CampaignVehicleConfiguration : IEntityTypeConfiguration<CampaignVehicle>
    {
        public void Configure(EntityTypeBuilder<CampaignVehicle> builder)
        {
            builder.ToTable("CampaignVehicles");
            builder.HasKey(cv => cv.CampaignVehicleId);
            builder.Property(cv => cv.CampaignVehicleId).ValueGeneratedOnAdd();
            builder.Property(cv => cv.CampaignId);
            builder.Property(cv => cv.Vin);
            builder.Property(cv => cv.NotifyToken);
            builder.Property(cv => cv.NotifiedAt);
            builder.Property(cv => cv.ConfirmedAt);
            builder.Property(cv => cv.CompletedAt);
            builder.Property(cv => cv.Status);

            builder.HasOne(cv => cv.Campaign)
                   .WithMany(c => c.CampaignVehicles)
                   .HasForeignKey(cv => cv.CampaignId)
                   .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(cv => cv.Vehicle)
                   .WithMany(v => v.CampaignVehicles)
                   .HasForeignKey(cv => cv.Vin)
                   .OnDelete(DeleteBehavior.NoAction);
        }
    }
}
