using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OEMEVWarrantyManagement.Domain.Entities;

namespace OEMEVWarrantyManagement.Infrastructure.Persistence.EntityConfigurations
{
    public class VehiclePartHistoryConfiguration : IEntityTypeConfiguration<VehiclePartHistory>
    {
        public void Configure(EntityTypeBuilder<VehiclePartHistory> builder)
        {
            builder.ToTable("VehiclePartHistories");
            builder.HasKey(h => h.VehiclePartHistoryId);
            builder.Property(h => h.VehiclePartHistoryId).ValueGeneratedOnAdd();
            builder.Property(h => h.Vin).IsRequired(false); // allow null
            builder.Property(h => h.Model).IsRequired();
            builder.Property(h => h.SerialNumber).IsRequired();
            builder.Property(h => h.InstalledAt);
            builder.Property(h => h.UninstalledAt);
            builder.Property(h => h.ProductionDate).IsRequired();
            builder.Property(h => h.WarrantyPeriodMonths).IsRequired();
            builder.Property(h => h.WarrantyEndDate).IsRequired();
            builder.Property(h => h.ServiceCenterId).IsRequired();
            builder.Property(h => h.Condition).HasMaxLength(50).IsRequired();
            builder.Property(h => h.Status).HasMaxLength(50).IsRequired();
            builder.Property(h => h.Note).HasMaxLength(500);

            builder.HasIndex(h => new { h.Vin, h.SerialNumber });

            builder.HasOne(h => h.Vehicle)
                   .WithMany(v => v.VehiclePartHistories)
                   .HasForeignKey(h => h.Vin)
                   .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(h => h.ServiceCenter)
                   .WithMany()
                   .HasForeignKey(h => h.ServiceCenterId)
                   .OnDelete(DeleteBehavior.Restrict);
        }
    }
}