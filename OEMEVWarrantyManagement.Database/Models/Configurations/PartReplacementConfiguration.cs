using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace OEMEVWarrantyManagement.Database.Models.Configurations
{
    public class PartReplacementConfiguration : IEntityTypeConfiguration<PartsReplacement>
    {
        public void Configure(EntityTypeBuilder<PartsReplacement> builder)
        {
            builder.ToTable("PartsReplacement");
            builder.HasKey(p => p.SerialNumber);
            builder.Property(p => p.SerialNumber).HasMaxLength(100);
            // Relationships
            builder.HasOne(p => p.PartTypeModel)
                   .WithMany(ptm => ptm.PartsReplacements)
                   .HasForeignKey(p => p.PartTypeModelId)
                   .IsRequired();

            builder.HasOne(p => p.Warranty)
                   .WithMany(w => w.PartsReplacements)
                   .HasForeignKey(p => p.WarrantyId);

            builder.HasOne(p => p.RecallHistory)
                   .WithMany(rh => rh.PartsReplacements)
                   .HasForeignKey(p => p.RecallHistoryId);
        }
    }
}
