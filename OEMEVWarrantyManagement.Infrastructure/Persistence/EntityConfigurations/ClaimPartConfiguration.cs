using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OEMEVWarrantyManagement.Domain.Entities;


namespace OEMEVWarrantyManagement.Infrastructure.Persistence.EntityConfigurations
{
    public class ClaimPartConfiguration : IEntityTypeConfiguration<ClaimPart>
    {
        public void Configure(EntityTypeBuilder<ClaimPart> builder)
        {
            builder.ToTable("ClaimParts");
            builder.HasKey(cp => cp.ClaimPartId);
            builder.Property(cp => cp.ClaimPartId).ValueGeneratedOnAdd();
            builder.Property(cp => cp.ClaimId);
            builder.Property(cp => cp.PartId);
            builder.Property(cp => cp.SerialNumber);
            builder.Property(cp => cp.Action);
            builder.Property(cp => cp.Cost).HasColumnType("decimal(18,2)"); ;

            builder.HasOne(cp => cp.WarrantyClaim)
                   .WithMany(wc => wc.ClaimParts)
                   .HasForeignKey(cp => cp.ClaimId);

            builder.HasOne(cp => cp.Part)
                   .WithMany(p => p.ClaimParts)
                   .HasForeignKey(cp => cp.PartId);
        }
    }
}
