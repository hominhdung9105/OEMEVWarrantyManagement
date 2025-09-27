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
    public class ClaimAttachmentConfiguration : IEntityTypeConfiguration<ClaimAttachment>
    {
        public void Configure(EntityTypeBuilder<ClaimAttachment> builder)
        {
            builder.ToTable("ClaimAttachments");
            builder.HasKey(ca => ca.AttachmentId);
            builder.Property(ca => ca.AttachmentId).ValueGeneratedOnAdd();
            builder.Property(ca => ca.ClaimId);
            builder.Property(ca => ca.URL).IsRequired();
            builder.Property(ca => ca.UploadedBy);

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
