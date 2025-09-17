using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace OEMEVWarrantyManagement.Database.Models.Configurations
{
    public class RoleEmployeeConfiguration : IEntityTypeConfiguration<RoleEmployee>
    {
        public void Configure(EntityTypeBuilder<RoleEmployee> builder)
        {
            builder.ToTable("RoleEmployee");
            builder.HasKey(re => new { re.RoleId, re.EmployeeId }); // Composite primary key
            // Relationship with Role
            builder.HasOne(re => re.Role)
                   .WithMany(r => r.RoleEmployees)
                   .HasForeignKey(re => re.RoleId)
                   .OnDelete(DeleteBehavior.Restrict);
            // Relationship with Employee
            builder.HasOne(re => re.Employee)
                   .WithMany(e => e.RoleEmployees)
                   .HasForeignKey(re => re.EmployeeId)
                   .OnDelete(DeleteBehavior.Restrict);
        }
    }

}
