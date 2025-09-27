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
    public class CampaignTargetConfiguration : IEntityTypeConfiguration<CampaignTarget>
    {
        public void Configure(EntityTypeBuilder<CampaignTarget> builder)
        {
            builder.ToTable("CampaignTargets");
            builder.HasKey(ct => ct.CampaignTargetId);
            builder.Property(ct => ct.CampaignTargetId).ValueGeneratedOnAdd();
            builder.Property(ct => ct.CampaignId);
            builder.Property(ct => ct.TargetType);
            builder.Property(ct => ct.TargetRefId);
            builder.Property(ct => ct.YearFrom);
            builder.Property(ct => ct.YearTo);

            builder.HasOne(ct => ct.Campaign)
                   .WithMany(c => c.CampaignTargets)
                   .HasForeignKey(ct => ct.CampaignId);
        }
    }
}
