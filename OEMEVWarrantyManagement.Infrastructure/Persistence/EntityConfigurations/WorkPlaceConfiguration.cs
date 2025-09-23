using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OEMEVWarrantyManagement.Domain.Entities;

namespace OEMEVWarrantyManagement.Infrastructure.Persistence.EntityConfigurations
{
    public class WorkPlaceConfiguration : IEntityTypeConfiguration<WorkPlaces>
    {
        public void Configure(EntityTypeBuilder<WorkPlaces> builder)
        {
            builder.ToTable("WorkPlaces");
            builder.HasKey(wp => wp.Id);
            builder.Property(wp => wp.Name).IsRequired().HasMaxLength(100);
            builder.Property(wp => wp.Location).IsRequired().HasMaxLength(200);

        }

    }
}
