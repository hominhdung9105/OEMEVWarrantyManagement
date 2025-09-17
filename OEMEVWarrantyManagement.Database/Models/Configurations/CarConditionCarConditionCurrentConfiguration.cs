using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace OEMEVWarrantyManagement.Database.Models.Configurations
{
    public class CarConditionCarConditionCurrentConfiguration : IEntityTypeConfiguration<CarConditionCarConditionCurrent>
    {
        public void Configure(EntityTypeBuilder<CarConditionCarConditionCurrent> builder)
        {
            builder.ToTable("CarConditionCarConditionCurrent");
            builder.HasKey(cc => new { cc.CarConditionId, cc.CarConditionCurrentId });

            builder.HasOne(ccccc => ccccc.CarCondition)
                   .WithMany(cc => cc.CarConditionCarConditionCurrents)
                   .HasForeignKey(ccccc => ccccc.CarConditionId);

            builder.HasOne(ccccc => ccccc.CarConditionCurrent)
                   .WithMany(ccc => ccc.CarConditionCarConditionCurrents)
                   .HasForeignKey(ccccc => ccccc.CarConditionCurrentId);
        }
    }
}
