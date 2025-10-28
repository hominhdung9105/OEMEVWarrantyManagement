using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OEMEVWarrantyManagement.Domain.Entities;


namespace OEMEVWarrantyManagement.Infrastructure.Persistence.EntityConfigurations
{
    public class CampaignConfiguration : IEntityTypeConfiguration<Campaign>
    {
        public void Configure(EntityTypeBuilder<Campaign> builder)
        {
            builder.ToTable("Campaigns");
            builder.HasKey(c => c.CampaignId);
            builder.Property(c => c.CampaignId).ValueGeneratedOnAdd();
            builder.Property(c => c.Title).IsRequired();
            builder.Property(c => c.Type);
            builder.Property(c => c.Description);
            builder.Property(c => c.StartDate);
            builder.Property(c => c.EndDate);
            builder.Property(c => c.Status);

            //builder.HasOne(c => c.Organization)
            //       .WithMany(o => o.Campaigns)
            //       .HasForeignKey(c => c.OrgId)
            //       .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
