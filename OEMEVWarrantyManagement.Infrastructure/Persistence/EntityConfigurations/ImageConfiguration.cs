using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OEMEVWarrantyManagement.Domain.Entities;

namespace OEMEVWarrantyManagement.Infrastructure.Persistence.EntityConfigurations
{
    public class ImageConfiguration : IEntityTypeConfiguration<Image>
    {
        public void Configure(EntityTypeBuilder<Image> builder)
        {
            builder.ToTable("Images");
            builder.HasKey(i => new { i.CarConditionCurrentId, i.FilePath });
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
