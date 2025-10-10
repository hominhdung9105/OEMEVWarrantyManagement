using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OEMEVWarrantyManagement.Domain.Entities;


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
            builder.Property(wc => wc.ComfirmStatus);
            builder.Property(wc => wc.ApprovedDate);
            builder.Property(wc => wc.failureDesc);
            builder.Property(wc => wc.PolicyId).HasColumnName("PolicyId");


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
                   .HasForeignKey(wc => wc.ComfirmStatus)
                   .IsRequired(false) // Important for nullable FK
                   .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(wc => wc.WarrantyPolicy)      
                   .WithMany(p => p.WarrantyClaims)      
                   .HasForeignKey(wc => wc.PolicyId)     
                   .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
