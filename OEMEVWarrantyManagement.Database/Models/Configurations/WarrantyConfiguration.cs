using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace OEMEVWarrantyManagement.Database.Models.Configurations
{
    public class WarrantyConfiguration : IEntityTypeConfiguration<Warranty>
    {
        public void Configure(EntityTypeBuilder<Warranty> builder)
        {
            builder.ToTable("Warranties");
            builder.HasKey(w => w.Id);
            builder.Property(w => w.Status)
                   .IsRequired()
                   .HasMaxLength(50);
            builder.Property(w => w.EmployeeTechId)
                   .IsRequired();
            builder.Property(w => w.RequestWarrantyId)
                   .IsRequired();
            builder.Property(w => w.StartDate)
                   .IsRequired();
            builder.Property(w => w.EndDate)
                   .IsRequired();
            builder.Property(w => w.EmployeeSCStaffId)
                   .IsRequired();
            builder.Property(w => w.PartRereplacementId)
                   .IsRequired();
            builder.Property(w => w.WarrantyRecordId)
                   .IsRequired();

            builder.HasOne(w => w.EmployeeSCStaff)
                   .WithMany(e => e.WarrantiesAsSCTech)
                   .HasForeignKey(w => w.EmployeeSCStaffId)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(w => w.RequestWarranty)
                   .WithMany(rw => rw.Warranties)
                   .HasForeignKey(w => w.RequestWarrantyId)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(w => w.EmployeeTech)
                   .WithMany(t => t.Warranties)
                   .HasForeignKey(w => w.EmployeeTechId)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(w => w.WarrantyRecord)
                   .WithMany(wr => wr.Warrantys)
                   .HasForeignKey(w => w.WarrantyRecordId)
                   .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
