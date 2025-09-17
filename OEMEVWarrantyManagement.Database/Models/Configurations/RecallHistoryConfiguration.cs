using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace OEMEVWarrantyManagement.Database.Models.Configurations
{
    public class RecallHistoryConfiguration : IEntityTypeConfiguration<RecallHistory>
    {
        public void Configure(EntityTypeBuilder<RecallHistory> builder)
        {
            builder.ToTable("RecallHistories");
            builder.HasKey(rh => rh.Id);
            builder.Property(rh => rh.Id).IsRequired();
            builder.Property(rh => rh.VIN).IsRequired();
            builder.Property(rh => rh.Status).IsRequired();
            builder.Property(rh => rh.DateStart).IsRequired();
            builder.Property(rh => rh.DateEnd).IsRequired();

            builder.HasOne(rh => rh.EmployeeTechs)
                   .WithMany(t => t.RecallHistoriesAsTechs)
                   .HasForeignKey(rh => rh.EmployeeSCTechId)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(rh => rh.EmployeeStaffs)
                   .WithMany(e => e.RecallHistoriesAsSCStaff)
                   .HasForeignKey(rh => rh.EmpoloyeeSCStaffId)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(rh => rh.CarInfo)
                   .WithMany(ci => ci.RecallHistories)
                   .HasForeignKey(rh => rh.VIN)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(rh => rh.Recall)
                   .WithMany(r => r.RecallHistories)
                   .HasForeignKey(rh => rh.RecallId)
                   .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
