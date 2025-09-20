using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace OEMEVWarrantyManagement.Database.Models.Configurations
{
    public class EmployeeConfiguration : IEntityTypeConfiguration <Employee>
    {
        public void Configure(EntityTypeBuilder<Employee> builder)
        {
            builder.ToTable("Employee");
            builder.HasKey(e => e.Id);
            builder.Property(e => e.Username).IsRequired().HasMaxLength(20);//need env variable
            builder.Property(e => e.Password).IsRequired().HasMaxLength(20);
            builder.Property(e => e.FullName).IsRequired().HasMaxLength(50);

            // Relationship with WorkPlace 1-N
            builder.HasOne(e => e.WorkPlaces)
                   .WithMany(wp => wp.Employees)
                   .HasForeignKey(e => e.WorkPlacesId);

            builder.HasOne(e => e.Role) 
                   .WithMany(r => r.Employees)
                   .HasForeignKey(e => e.RoleId)
                   .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
