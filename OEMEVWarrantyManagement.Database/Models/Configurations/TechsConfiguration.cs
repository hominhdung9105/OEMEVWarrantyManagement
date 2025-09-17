using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace OEMEVWarrantyManagement.Database.Models.Configurations
{
    public class TechsConfiguration : IEntityTypeConfiguration<Techs>
    {
        public void Configure(EntityTypeBuilder<Techs> builder)
        {
            builder.ToTable("Techs");
            builder.HasKey(t => t.Id);
            builder.Property(t => t.EmployeeId)
                   .IsRequired()
                   .HasMaxLength(50);

            builder.HasOne(t => t.Employee)
                   .WithMany(e => e.Teches)
                   .HasForeignKey(t => t.EmployeeId)
                   .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
