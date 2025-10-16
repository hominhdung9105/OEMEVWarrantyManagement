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
    public class BackWarrantyClaimConfiguration : IEntityTypeConfiguration<BackWarrantyClaim>
    {
        public void Configure(EntityTypeBuilder<BackWarrantyClaim> builder)
        {
            builder.ToTable("BackWarrantyClaim");
            builder.HasKey(f => new { f.WarrantyClaimId, f.CreatedDate });
            builder.Property(f => f.Description)
                .IsRequired()
                .HasMaxLength(1000);
            builder.Property(f => f.CreatedDate)
                .HasDefaultValueSql("GETDATE()");
            //1-N: Một WarrantyClaim có thể có nhiều Feedback
            builder.HasOne(f => f.WarrantyClaim)
                   .WithMany(wc => wc.Feedbacks)
                   .HasForeignKey(f => f.WarrantyClaimId)
                   .OnDelete(DeleteBehavior.Cascade);
            //1-N: Một Employee có thể tạo nhiều Feedback
            builder.HasOne(f => f.CreatedByEmployee)
                   .WithMany(e => e.CreatedFeedbacks)
                   .HasForeignKey(f => f.CreatedByEmployeeId)
                   .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
