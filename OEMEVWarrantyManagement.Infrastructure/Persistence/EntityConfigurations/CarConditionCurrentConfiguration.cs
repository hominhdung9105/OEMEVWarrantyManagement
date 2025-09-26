using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OEMEVWarrantyManagement.Domain.Entities;

namespace OEMEVWarrantyManagement.Infrastructure.Persistence.EntityConfigurations
{
    public class CarConditionCurrentConfiguration : IEntityTypeConfiguration<CarConditionCurrent>
    {
        public void Configure(EntityTypeBuilder<CarConditionCurrent> builder)
        {
            builder.ToTable("CarConditionCurrents");
            builder.HasKey(ccc => ccc.WarrantyRequestId);
            builder.Property(ccc => ccc.WarrantyRequestId).ValueGeneratedOnAdd();
            builder.Property(ccc => ccc.Condition)
                   .HasMaxLength(100);
            builder.Property(ccc => ccc.Detail)
                   .HasMaxLength(1000);
            builder.Property(ccc => ccc.TechnicianId);

            builder.HasOne(ccc => ccc.EmployeeTechnician)
                   .WithMany(e => e.CarConditionCurrents)
                   .HasForeignKey(ccc => ccc.TechnicianId)
                   .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
