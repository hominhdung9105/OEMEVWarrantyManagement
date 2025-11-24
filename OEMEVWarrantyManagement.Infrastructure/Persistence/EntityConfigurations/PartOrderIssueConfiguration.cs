using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OEMEVWarrantyManagement.Domain.Entities;

namespace OEMEVWarrantyManagement.Infrastructure.Persistence.EntityConfigurations
{
    public class PartOrderIssueConfiguration : IEntityTypeConfiguration<PartOrderIssue>
    {
        public void Configure(EntityTypeBuilder<PartOrderIssue> builder)
        {
            builder.ToTable("PartOrderIssues");
            builder.HasKey(i => i.IssueId);
            builder.Property(i => i.IssueId).ValueGeneratedOnAdd();
            builder.Property(i => i.OrderId).IsRequired();
            builder.Property(i => i.IssueType).IsRequired().HasMaxLength(50);
            builder.Property(i => i.Reason).IsRequired().HasMaxLength(100);
            builder.Property(i => i.ReasonDetail).HasMaxLength(500);
            builder.Property(i => i.Note).HasMaxLength(1000);
            builder.Property(i => i.CreatedBy).IsRequired();
            builder.Property(i => i.CreatedAt).IsRequired();

            builder.HasOne(i => i.PartOrder)
                   .WithMany(po => po.Issues)
                   .HasForeignKey(i => i.OrderId)
                   .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(i => i.CreatedByEmployee)
                   .WithMany()
                   .HasForeignKey(i => i.CreatedBy)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.HasIndex(i => i.OrderId);
        }
    }

    public class PartOrderDiscrepancyResolutionConfiguration : IEntityTypeConfiguration<PartOrderDiscrepancyResolution>
    {
        public void Configure(EntityTypeBuilder<PartOrderDiscrepancyResolution> builder)
        {
            builder.ToTable("PartOrderDiscrepancyResolutions");
            builder.HasKey(r => r.ResolutionId);
            builder.Property(r => r.ResolutionId).ValueGeneratedOnAdd();
            builder.Property(r => r.OrderId).IsRequired();
            builder.Property(r => r.Status).IsRequired().HasMaxLength(50);
            builder.Property(r => r.ResponsibleParty).HasMaxLength(50);
            builder.Property(r => r.Decision).HasMaxLength(1000);
            builder.Property(r => r.Note).HasMaxLength(1000);
            builder.Property(r => r.ResolvedBy);
            builder.Property(r => r.ResolvedAt);
            builder.Property(r => r.CreatedAt).IsRequired();

            builder.HasOne(r => r.PartOrder)
                   .WithOne(po => po.DiscrepancyResolution)
                   .HasForeignKey<PartOrderDiscrepancyResolution>(r => r.OrderId)
                   .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(r => r.ResolvedByEmployee)
                   .WithMany()
                   .HasForeignKey(r => r.ResolvedBy)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.HasIndex(r => r.OrderId).IsUnique();
            builder.HasIndex(r => r.Status);
        }
    }
}
