using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OEMEVWarrantyManagement.Database.Models.Configurations
{
    public class RecallHistoryPartsReplacementConfiguration : IEntityTypeConfiguration<RecallHistoryPartsReplacement>
    {
        public void Configure(EntityTypeBuilder<RecallHistoryPartsReplacement> builder)
        {
            builder.ToTable("RecallHistoryPartsReplacements");
            builder.HasKey(rhpr => new { rhpr.RecallHistoryId, rhpr.PartsReplacementId });

            //relationships with RecallHistory
            builder.HasOne(rhpr => rhpr.RecallHistory)
                   .WithMany(rh => rh.RecallHistoryPartsReplacements)
                   .HasForeignKey(rhpr => rhpr.RecallHistoryId);
            //relationship with PartsReplacement
            builder.HasOne(rhpr => rhpr.PartsReplacement)
                   .WithMany(pr => pr.RecallHistoryPartsReplacements)
                   .HasForeignKey(rhpr => rhpr.PartsReplacementId);
        }
    }
}
