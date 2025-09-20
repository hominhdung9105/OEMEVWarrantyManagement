using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace OEMEVWarrantyManagement.Database.Models.Configurations
{
    public class RecallPartTypeModelConfiguration : IEntityTypeConfiguration<RecallPartTypeModel>
    {
        public void Configure(EntityTypeBuilder<RecallPartTypeModel> builder)
        {
            builder.ToTable("RecallPartsReplacements");
            builder.HasKey(rpr => new { rpr.RecallId, rpr.PartTypeModelId });

            //relationships with Recall and PartsReplacement
            builder.HasOne(rpr => rpr.Recall)
                   .WithMany(r => r.RecallPartTypeModels)
                   .HasForeignKey(rpr => rpr.RecallId);
            //relationships with PartsReplacement and Recall
            builder.HasOne(rpr => rpr.PartTypeModel)
                   .WithMany(pr => pr.RecallPartTypeModels)
                   .HasForeignKey(rpr => rpr.PartTypeModelId);

        }
    }
}
