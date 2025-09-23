using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OEMEVWarrantyManagement.Domain.Entities;

namespace OEMEVWarrantyManagement.Infrastructure.Persistence.EntityConfigurations
{
    public class RecallConfiguration : IEntityTypeConfiguration<Recall>
    {
        public void Configure(EntityTypeBuilder<Recall> builder)
        {
            builder.ToTable("Recall");
            builder.HasKey(r => r.Id);
            builder.Property(r => r.EVMStaffId).IsRequired();
            builder.Property(r => r.Name).IsRequired();
            builder.Property(r => r.Status).IsRequired();
            builder.Property(r => r.Detail);
            builder.Property(r => r.PartReplacementId).IsRequired();
            builder.Property(r => r.NumberOfCars).IsRequired();

        }
    }
}
