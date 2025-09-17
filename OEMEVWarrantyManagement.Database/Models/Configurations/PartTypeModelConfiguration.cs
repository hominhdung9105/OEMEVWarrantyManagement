using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace OEMEVWarrantyManagement.Database.Models.Configurations
{
    public class PartTypeModelConfiguration : IEntityTypeConfiguration<PartTypeModel>
    {
        public void Configure(EntityTypeBuilder<PartTypeModel> builder)
        {
            builder.ToTable("PartTypeModels");
            builder.HasKey(ptm => ptm.Id);
            builder.Property(ptm => ptm.Name)
                   .IsRequired()
                   .HasMaxLength(100);
            builder.Property(ptm => ptm.PartTypeId)
                   .IsRequired();

            // Configure the relationship between PartTypeModel and PartType 1-N
            builder.HasOne(ptm => ptm.PartType)
                     .WithMany(pt => pt.PartTypeModels)
                     .HasForeignKey(ptm => ptm.PartTypeId);
        }
    }
}
