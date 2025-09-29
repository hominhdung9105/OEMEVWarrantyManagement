using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OEMEVWarrantyManagement.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
            builder.Property(cv => cv.NotifiedDate);
            builder.Property(cv => cv.HandledDate);
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
