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
            builder.HasKey(ccc => ccc.Id);
            builder.Property(ccc => ccc.Id).ValueGeneratedOnAdd();
            builder.Property(ccc => ccc.Condition)
                   .IsRequired()
                   .HasMaxLength(100);
            builder.Property(ccc => ccc.Detail)
                   .HasMaxLength(1000);
            builder.Property(ccc => ccc.TechnicianId)
                   .IsRequired();

            builder.HasOne(ccc => ccc.EmployeeTechnician)
                   .WithMany(e => e.CarConditionCurrents)
                   .HasForeignKey(ccc => ccc.TechnicianId);
        }
    }
}
