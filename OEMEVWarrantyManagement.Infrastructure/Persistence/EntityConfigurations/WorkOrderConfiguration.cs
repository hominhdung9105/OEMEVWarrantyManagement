using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OEMEVWarrantyManagement.Domain.Entities;


namespace OEMEVWarrantyManagement.Infrastructure.Persistence.EntityConfigurations
{
    public class WorkOrderConfiguration : IEntityTypeConfiguration<WorkOrder>
    {
        public void Configure(EntityTypeBuilder<WorkOrder> builder)
        {
            builder.ToTable("WorkOrders");
            builder.HasKey(wo => wo.WorkOrderId);
            builder.Property(wo => wo.WorkOrderId).ValueGeneratedOnAdd();
            builder.Property(wo => wo.AssignedTo);
            builder.Property(wo => wo.Type);
            builder.Property(wo => wo.Target);
            builder.Property(wo => wo.TargetId);
            builder.Property(wo => wo.Status);
            builder.Property(wo => wo.StartDate);
            builder.Property(wo => wo.EndDate);

            builder.HasOne(wo => wo.AssignedToEmployee)
                   .WithMany(e => e.AssignedWorkOrders)
                   .HasForeignKey(wo => wo.AssignedTo)
                   .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
