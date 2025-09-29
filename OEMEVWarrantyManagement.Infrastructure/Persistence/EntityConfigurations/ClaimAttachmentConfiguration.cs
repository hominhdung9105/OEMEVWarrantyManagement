using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OEMEVWarrantyManagement.Domain.Entities;

namespace OEMEVWarrantyManagement.Infrastructure.Persistence.EntityConfigurations
{
    public class ClaimAttachmentConfiguration : IEntityTypeConfiguration<ClaimAttachment>
    {
        public void Configure(EntityTypeBuilder<ClaimAttachment> builder)
        {
            builder.ToTable("ClaimAttachments");
            builder.HasKey(ca => ca.AttachmentId);
            builder.Property(ca => ca.AttachmentId).ValueGeneratedOnAdd();
            builder.Property(ca => ca.ClaimId).IsRequired();
            builder.Property(ca => ca.URL).IsRequired();
            builder.Property(ca => ca.UploadedBy).IsRequired();

            builder.HasOne(ca => ca.WarrantyClaim)
                   .WithMany(wc => wc.ClaimAttachments)
                   .HasForeignKey(ca => ca.ClaimId);

            builder.HasOne(ca => ca.UploadedByEmployee)
                   .WithMany(e => e.UploadedAttachments)
                   .HasForeignKey(ca => ca.UploadedBy)
                   .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
