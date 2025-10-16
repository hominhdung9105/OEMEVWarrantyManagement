using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OEMEVWarrantyManagement.Domain.Entities;


namespace OEMEVWarrantyManagement.Infrastructure.Persistence.EntityConfigurations
{
    public class EmployeeConfiguration : IEntityTypeConfiguration<Employee>
    {
        public void Configure(EntityTypeBuilder<Employee> builder)
        {
            builder.ToTable("Employee");
            builder.HasKey(e => e.UserId);
            builder.Property(e => e.UserId).ValueGeneratedOnAdd();
            builder.Property(e => e.Email).IsRequired();
            builder.Property(e => e.PasswordHash).IsRequired();
            builder.Property(e => e.Role).IsRequired();
            builder.Property(e => e.OrgId);

            builder.HasOne(e => e.Organization)
                   .WithMany(o => o.Employees)
                   .HasForeignKey(e => e.OrgId)
                   .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
