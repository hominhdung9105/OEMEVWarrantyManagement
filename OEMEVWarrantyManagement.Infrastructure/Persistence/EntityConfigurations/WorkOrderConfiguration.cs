using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OEMEVWarrantyManagement.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OEMEVWarrantyManagement.Infrastructure.Persistence.EntityConfigurations
{
    public class WorkOrderConfiguration : IEntityTypeConfiguration<WorkOrder>
    {
        public void Configure(EntityTypeBuilder<WorkOrder> builder)
        {
            builder.ToTable("WorkOrders");
            builder.HasKey(wo => wo.WorkOrderId);
            builder.Property(wo => wo.WorkOrderId).ValueGeneratedOnAdd();
            builder.Property(wo => wo.ClaimId);
            builder.Property(wo => wo.AssignedTo);
            builder.Property(wo => wo.Type);
            builder.Property(wo => wo.TargetId);
            builder.Property(wo => wo.Status);
            builder.Property(wo => wo.StartDate);
            builder.Property(wo => wo.EndDate);
            builder.Property(wo => wo.Notes);

            builder.HasOne(wo => wo.WarrantyClaim)
                   .WithMany(wc => wc.WorkOrders)
                   .HasForeignKey(wo => wo.ClaimId)
                   .IsRequired(false);

            builder.HasOne(wo => wo.AssignedToEmployee)
                   .WithMany(e => e.AssignedWorkOrders)
                   .HasForeignKey(wo => wo.AssignedTo)
                   .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
