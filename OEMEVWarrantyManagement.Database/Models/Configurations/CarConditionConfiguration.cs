using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace OEMEVWarrantyManagement.Database.Models.Configurations
{
    public class CarConditionConfiguration : IEntityTypeConfiguration<CarCondition>
    {
        public void Configure(EntityTypeBuilder<CarCondition> builder)
        {
            builder.ToTable("CarConditions");
            builder.HasKey(cc => cc.Id);
            builder.Property(cc => cc.Name)
                   .IsRequired()
                   .HasMaxLength(100);
            builder.Property(cc => cc.PartTypeModelId)
                   .IsRequired();

            // Relationship with PartTypeModel 1-N
            builder.HasOne(cc => cc.PartTypeModel)
                   .WithMany(ptm => ptm.CarConditions)
                   .HasForeignKey(cc => cc.PartTypeModelId);
        }
    }
}
