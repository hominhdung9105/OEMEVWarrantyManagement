using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OEMEVWarrantyManagement.Domain.Entities;


namespace OEMEVWarrantyManagement.Infrastructure.Persistence.EntityConfigurations
{
    public class OrganizationConfiguration : IEntityTypeConfiguration<Organization>
    {
        public void Configure(EntityTypeBuilder<Organization> builder)
        {
            builder.ToTable("Organizations");
            builder.HasKey(o => o.OrgId);
            builder.Property(o => o.OrgId).ValueGeneratedOnAdd();
            builder.Property(o => o.Name).IsRequired();
            builder.Property(o => o.Type).IsRequired();
            builder.Property(o => o.Region);
            builder.Property(o => o.ContactInfo);
        }
    }
}
