using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OEMEVWarrantyManagement.Domain.Entities;

namespace OEMEVWarrantyManagement.Infrastructure.Persistence.EntityConfigurations
{
    public class CampaignNotificationConfiguration : IEntityTypeConfiguration<CampaignNotification>
    {
        public void Configure(EntityTypeBuilder<CampaignNotification> builder)
        {
            builder.ToTable("CampaignNotifications");
            builder.HasKey(cn => cn.CampaignNotificationId);
            builder.Property(cn => cn.CampaignNotificationId).ValueGeneratedOnAdd();

            builder.Property(cn => cn.CampaignId).IsRequired();
            builder.Property(cn => cn.Vin).IsRequired();
            builder.Property(cn => cn.EmailSentCount).IsRequired().HasDefaultValue(0);
            builder.Property(cn => cn.IsCompleted).IsRequired().HasDefaultValue(false);
            builder.Property(cn => cn.CreatedAt).IsRequired();

            // Foreign keys
            builder.HasOne(cn => cn.Campaign)
                   .WithMany()
                   .HasForeignKey(cn => cn.CampaignId)
                   .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(cn => cn.Vehicle)
                   .WithMany()
                   .HasForeignKey(cn => cn.Vin)
                   .OnDelete(DeleteBehavior.Restrict);

            // Index for efficient queries
            builder.HasIndex(cn => new { cn.CampaignId, cn.Vin }).IsUnique();
            builder.HasIndex(cn => cn.IsCompleted);
            builder.HasIndex(cn => cn.LastEmailSentAt);
        }
    }
}
