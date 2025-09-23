using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OEMEVWarrantyManagement.Domain.Entities;

namespace OEMEVWarrantyManagement.Infrastructure.Persistence.EntityConfigurations
{
    public class WarrantyRecordConfiguration : IEntityTypeConfiguration<WarrantyRecord>
    {
            public void Configure(EntityTypeBuilder<WarrantyRecord> builder)
            {
            builder.ToTable("WarrantyRecord");
            builder.HasKey(wr => wr.Id);
            builder.Property(wr => wr.Id).ValueGeneratedOnAdd();
            builder.Property(wr => wr.StartDate).IsRequired();
            builder.Property(wr => wr.EndDate).IsRequired();
            builder.Property(wr => wr.CustomerId).IsRequired();
            builder.Property(wr => wr.VIN).IsRequired();
            builder.Property(wr => wr.WarrantyPolicyId).IsRequired();

            builder.HasOne(wr => wr.WarrantyPolicy)
                    .WithMany(wp => wp.WarrantyRecords)
                    .HasForeignKey(wr => wr.WarrantyPolicyId);

            builder.HasOne(wr => wr.CarInfo)
                    .WithMany(ci => ci.WarrantyRecords)
                    .HasForeignKey(wr => wr.VIN);

            builder.HasOne(wr => wr.Customer)
                   .WithMany(c => c.WarrantyRecords)
                   .HasForeignKey(wr => wr.CustomerId)
                   .OnDelete(DeleteBehavior.Restrict);


        }
    }
}
