using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OEMEVWarrantyManagement.Domain.Entities;

namespace OEMEVWarrantyManagement.Infrastructure.Persistence.EntityConfigurations
{
    public class AppointmentConfiguration : IEntityTypeConfiguration<Appointment>
    {
        public void Configure(EntityTypeBuilder<Appointment> builder)
        {
            builder.ToTable("Appointments");
            builder.HasKey(a => a.AppointmentId);
            builder.Property(a => a.AppointmentId).ValueGeneratedOnAdd();

            builder.Property(a => a.AppointmentType).IsRequired();
            builder.Property(a => a.CustomerId).IsRequired();
            builder.Property(a => a.ServiceCenterId).IsRequired();
            builder.Property(a => a.AppointmentDate).HasColumnType("date");
            builder.Property(a => a.Slot).IsRequired();
            builder.Property(a => a.Status).IsRequired();
            builder.Property(a => a.CreatedAt).HasColumnType("datetime2");
            builder.Property(a => a.Note);

            builder.HasOne(a => a.Customer)
                   .WithMany()
                   .HasForeignKey(a => a.CustomerId)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(a => a.ServiceCenter)
                   .WithMany()
                   .HasForeignKey(a => a.ServiceCenterId)
                   .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
