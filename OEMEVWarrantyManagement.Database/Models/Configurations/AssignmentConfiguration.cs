using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace OEMEVWarrantyManagement.Database.Models.Configurations
{
    public class AssignmentConfiguration : IEntityTypeConfiguration<Assignment>
    {
        public void Configure(EntityTypeBuilder<Assignment> builder)
        {
            builder.ToTable("Assignment");
            builder.HasKey(a => a.Id);
            builder.Property(a => a.TaskId).IsRequired();
            builder.Property(a => a.Status).IsRequired();
            builder.Property(a => a.AssignedEndDate).IsRequired();

            builder.HasOne(a => a.EmployeeSCStaff)
                   .WithMany(e => e.AssignmentsAsSCStaff)
                   .HasForeignKey(a => a.SCStaffId)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(a => a.EmployeeSCTech)
                   .WithMany(e => e.AssignmentsAsSCTech)
                   .HasForeignKey(a => a.SCTechID)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(a => a.Task)
                   .WithMany(t => t.Assignments)
                   .HasForeignKey(a => a.TaskId);
        }
    }
}
