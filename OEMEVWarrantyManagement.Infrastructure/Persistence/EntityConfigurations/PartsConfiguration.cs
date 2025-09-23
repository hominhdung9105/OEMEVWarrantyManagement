using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OEMEVWarrantyManagement.Domain.Entities;

namespace OEMEVWarrantyManagement.Infrastructure.Persistence.EntityConfigurations
{
    public class PartsConfiguration : IEntityTypeConfiguration<Parts>
    {
        public void Configure(EntityTypeBuilder<Parts> builder)
        {
            builder.ToTable("Parts");
            builder.HasKey(p => p.Id);
            builder.Property(p => p.PartTypeModelId)
                   .IsRequired();
            builder.Property(p => p.Number)
                    .IsRequired()
                    .HasMaxLength(100);

            builder.HasOne(p => p.PartTypeModels)
                   .WithMany(pm => pm.Parts)
                   .HasForeignKey(p => p.PartTypeModelId)
                   .OnDelete(DeleteBehavior.Restrict);


            builder.HasOne(p => p.RequestPart)
                   .WithMany(rp => rp.Parts)
                   .HasForeignKey(p => p.RequestPartsId);


            builder.HasOne(p => p.DeliveryPart)
                   .WithMany(dp => dp.Parts)
                   .HasForeignKey(p => p.DeliveryPartId);
        }
    }
    
}
