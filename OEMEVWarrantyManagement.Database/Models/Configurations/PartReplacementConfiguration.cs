using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace OEMEVWarrantyManagement.Database.Models.Configurations
{
    public class PartReplacementConfiguration : IEntityTypeConfiguration<PartsReplacement>
    {
        public void Configure(EntityTypeBuilder<PartsReplacement> builder)
        {
            builder.ToTable("PartsReplacement");
            builder.HasKey(pr => pr.Id);
            builder.Property(pr => pr.PartModelId).IsRequired();
            // Relationships
            builder.HasOne(pr => pr.PartTypeModel)
                   .WithMany(ptm => ptm.PartsReplacements)
                   .HasForeignKey(pr => pr.PartModelId)
                   .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
