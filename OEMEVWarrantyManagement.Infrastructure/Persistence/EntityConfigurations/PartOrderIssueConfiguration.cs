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
            builder.Property(r => r.OverallNote).HasMaxLength(2000);
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

            builder.HasMany(r => r.Details)
                   .WithOne(d => d.Resolution)
                   .HasForeignKey(d => d.ResolutionId)
                   .OnDelete(DeleteBehavior.Cascade);

            builder.HasIndex(r => r.OrderId).IsUnique();
            builder.HasIndex(r => r.Status);
        }
    }

    public class PartOrderDiscrepancyDetailConfiguration : IEntityTypeConfiguration<PartOrderDiscrepancyDetail>
    {
        public void Configure(EntityTypeBuilder<PartOrderDiscrepancyDetail> builder)
        {
            builder.ToTable("PartOrderDiscrepancyDetails");
            builder.HasKey(d => d.DetailId);
            builder.Property(d => d.DetailId).ValueGeneratedOnAdd();
            builder.Property(d => d.ResolutionId).IsRequired();
            builder.Property(d => d.SerialNumber).IsRequired().HasMaxLength(100);
            builder.Property(d => d.Model).IsRequired().HasMaxLength(200);
            builder.Property(d => d.DiscrepancyType).IsRequired().HasMaxLength(50);
            builder.Property(d => d.ResponsibleParty).IsRequired().HasMaxLength(50);
            builder.Property(d => d.Action).IsRequired().HasMaxLength(100);
            builder.Property(d => d.Note).HasMaxLength(1000);

            builder.HasOne(d => d.Resolution)
                   .WithMany(r => r.Details)
                   .HasForeignKey(d => d.ResolutionId)
                   .OnDelete(DeleteBehavior.Cascade);

            builder.HasIndex(d => d.ResolutionId);
            builder.HasIndex(d => d.SerialNumber);
        }
    }
}
