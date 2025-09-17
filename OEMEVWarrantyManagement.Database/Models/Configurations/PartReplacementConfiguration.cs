using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace OEMEVWarrantyManagement.Database.Models.Configurations
{
    public class PartReplacementConfiguration : IEntityTypeConfiguration<PartReplacement>
    {
        public void Configure(EntityTypeBuilder<PartReplacement> builder)
        {
            builder.ToTable("PartReplacements");
            builder.HasKey(pr => pr.Id);
            builder.Property(pr => pr.PartTypeModelId).IsRequired();
            builder.Property(pr => pr.VIN).IsRequired();
            builder.Property(pr => pr.EmployeeId).IsRequired();

            builder.HasOne(pr => pr.PartTypeModel)
                   .WithMany(ptm => ptm.PartReplacements)
                   .HasForeignKey(pr => pr.PartTypeModelId);

            builder.HasOne(pr => pr.Employee)
                   .WithMany(e => e.PartReplacements)
                   .HasForeignKey(pr => pr.EmployeeId);
        }
    }
}
