using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace OEMEVWarrantyManagement.Database.Models.Configurations
{
    public class ImageConfiguration : IEntityTypeConfiguration<Image>
    {
        public void Configure(EntityTypeBuilder<Image> builder)
        {
            builder.ToTable("Images");
            builder.HasKey(i => i.Id);
            builder.Property(i => i.FilePath)
                   .IsRequired()
                   .HasMaxLength(200);
            builder.Property(i => i.CarConditionCurrentId)
                   .IsRequired();

            builder.HasOne(i => i.CarConditionCurrent)
                   .WithMany(ccc => ccc.Images)
                   .HasForeignKey(i => i.CarConditionCurrentId);
        }
    }
}
