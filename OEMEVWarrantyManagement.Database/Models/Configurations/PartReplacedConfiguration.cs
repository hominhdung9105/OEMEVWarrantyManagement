using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace OEMEVWarrantyManagement.Database.Models.Configurations
{
    public class PartReplacedConfiguration : IEntityTypeConfiguration<PartReplaced>
    {
        public void Configure(EntityTypeBuilder<PartReplaced> builder)
        {
            builder.ToTable("PartReplaced");
            builder.HasKey(pr => pr.Id);
            builder.Property(pr => pr.PartTypeModelId).IsRequired();
            builder.Property(pr => pr.VIN).IsRequired().HasMaxLength(17);
            builder.Property(pr => pr.EmployeeId).IsRequired();

            builder.HasOne(pr => pr.PartTypeModel)
                   .WithMany(ptm => ptm.PartReplaceds)
                   .HasForeignKey(pr => pr.PartTypeModelId)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(pr => pr.Employee)
                   .WithMany(e => e.PartReplaceds)
                   .HasForeignKey(pr => pr.EmployeeId)
                   .OnDelete(DeleteBehavior.Restrict);

        }
    }
}
