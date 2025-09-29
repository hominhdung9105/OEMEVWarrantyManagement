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
    public class WarrantyClaimConfiguration : IEntityTypeConfiguration<WarrantyClaim>
    {
        public void Configure(EntityTypeBuilder<WarrantyClaim> builder)
        {
            builder.ToTable("WarrantyClaims");
            builder.HasKey(wc => wc.ClaimId);

            builder.Property(wc => wc.ClaimId).ValueGeneratedOnAdd();
            builder.Property(wc => wc.Vin);
            builder.Property(wc => wc.ServiceCenterId);
            builder.Property(wc => wc.CreatedBy);
            builder.Property(wc => wc.CreatedDate);
            builder.Property(wc => wc.Status).IsRequired();
            builder.Property(wc => wc.Description);
            builder.Property(wc => wc.ApprovedBy);
            builder.Property(wc => wc.ApprovedDate);

            builder.HasOne(wc => wc.Vehicle)
                   .WithMany(v => v.WarrantyClaims)
                   .HasForeignKey(wc => wc.Vin)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(wc => wc.ServiceCenter)
                   .WithMany(o => o.ServicedWarrantyClaims)
                   .HasForeignKey(wc => wc.ServiceCenterId)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(wc => wc.CreatedByEmployee)
                   .WithMany(e => e.CreatedClaims)
                   .HasForeignKey(wc => wc.CreatedBy)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(wc => wc.ApprovedByEmployee)
                   .WithMany(e => e.ApprovedClaims)
                   .HasForeignKey(wc => wc.ApprovedBy)
                   .IsRequired(false) // Important for nullable FK
                   .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
