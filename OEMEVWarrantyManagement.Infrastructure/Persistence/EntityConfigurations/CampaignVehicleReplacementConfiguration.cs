using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OEMEVWarrantyManagement.Domain.Entities;

namespace OEMEVWarrantyManagement.Infrastructure.Persistence.EntityConfigurations
{
    public class CampaignVehicleReplacementConfiguration : IEntityTypeConfiguration<CampaignVehicleReplacement>
    {
        public void Configure(EntityTypeBuilder<CampaignVehicleReplacement> builder)
        {
            builder.ToTable("CampaignVehicleReplacements");
            builder.HasKey(x => x.CampaignVehicleReplacementId);
            builder.Property(x => x.CampaignVehicleReplacementId).ValueGeneratedOnAdd();
            builder.Property(x => x.CampaignVehicleId).IsRequired();
            builder.Property(x => x.OldSerial).IsRequired();
            builder.Property(x => x.NewSerial).IsRequired();
            builder.Property(x => x.ReplacedAt).IsRequired();

            builder.HasOne(x => x.CampaignVehicle)
                   .WithMany(cv => cv.Replacements)
                   .HasForeignKey(x => x.CampaignVehicleId)
                   .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
