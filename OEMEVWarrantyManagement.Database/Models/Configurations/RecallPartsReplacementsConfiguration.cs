using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace OEMEVWarrantyManagement.Database.Models.Configurations
{
    public class RecallPartsReplacementsConfiguration : IEntityTypeConfiguration<RecallPartsReplacement>
    {
        public void Configure(EntityTypeBuilder<RecallPartsReplacement> builder)
        {
            builder.ToTable("RecallPartsReplacements");
            builder.HasKey(rpr => new { rpr.RecallId, rpr.PartsReplacementId });

            //relationships with Recall and PartsReplacement
            builder.HasOne(rpr => rpr.Recall)
                   .WithMany(r => r.RecallPartsReplacements)
                   .HasForeignKey(rpr => rpr.RecallId)
                   .OnDelete(DeleteBehavior.Cascade);
            //relationships with PartsReplacement and Recall
            builder.HasOne(rpr => rpr.PartsReplacement)
                   .WithMany(pr => pr.RecallPartsReplacements)
                   .HasForeignKey(rpr => rpr.PartsReplacementId)
                   .OnDelete(DeleteBehavior.Cascade);

        }
    }
}
