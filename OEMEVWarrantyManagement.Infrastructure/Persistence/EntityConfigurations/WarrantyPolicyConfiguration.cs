using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OEMEVWarrantyManagement.Domain.Entities;

namespace OEMEVWarrantyManagement.Infrastructure.Persistence.EntityConfigurations
{
    public class WarrantyPolicyConfiguration : IEntityTypeConfiguration<WarrantyPolicy>
    {
        public void Configure(EntityTypeBuilder<WarrantyPolicy> builder)
        {
            builder.ToTable("WarrantyPolicies");
            builder.HasKey(wp => wp.Id);
            builder.Property(wp => wp.PeriodInMonths).IsRequired();
            builder.Property(wp => wp.Coverage).IsRequired().HasMaxLength(500);
            builder.Property(wp => wp.Conditions).IsRequired().HasMaxLength(1000);
        }
    }
}
