using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OEMEVWarrantyManagement.Domain.Entities;

namespace OEMEVWarrantyManagement.Infrastructure.Persistence.EntityConfigurations
{
    public class PartTypeConfiguration : IEntityTypeConfiguration<PartType>
    {
        public void Configure(EntityTypeBuilder<PartType> builder)
        {
            builder.ToTable("PartTypes");
            builder.HasKey(pt => pt.Id);
            builder.Property(pt => pt.Name)
                   .IsRequired()
                   .HasMaxLength(100);
        }
    }
}
