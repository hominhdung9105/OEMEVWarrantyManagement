using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OEMEVWarrantyManagement.Domain.Entities;

namespace OEMEVWarrantyManagement.Infrastructure.Persistence.EntityConfigurations
{
    public class AssignmentConfiguration : IEntityTypeConfiguration<Assignment>
    {
        public void Configure(EntityTypeBuilder<Assignment> builder)
        {
            builder.ToTable("Assignment");
            builder.HasKey(a => a.Id);
            builder.Property(a => a.Id).ValueGeneratedOnAdd();
            builder.Property(a => a.Status).IsRequired();
            builder.Property(a => a.TaskName).IsRequired().HasMaxLength(200);
            builder.Property(a => a.TaskDescription).HasMaxLength(1000);
            builder.Property(a => a.StartDate).IsRequired();
            builder.Property(a => a.EndDate).IsRequired();

            builder.HasOne(a => a.EmployeeSCStaff)
                   .WithMany(e => e.AssignmentsAsSCStaff)
                   .HasForeignKey(a => a.SCStaffId)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(a => a.EmployeeSCTech)
                   .WithMany(e => e.AssignmentsAsSCTech)
                   .HasForeignKey(a => a.SCTechID)
                   .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
