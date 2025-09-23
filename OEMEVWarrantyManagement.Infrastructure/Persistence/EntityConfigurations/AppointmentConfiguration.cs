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
            builder.HasKey(a => a.Id);
            builder.Property(a => a.Id).ValueGeneratedOnAdd();
            builder.Property(a => a.EmployeeId).IsRequired();
            builder.Property(a => a.CustomerId).IsRequired();
            builder.Property(a => a.Note).HasMaxLength(500);
            builder.Property(a => a.DateTime).IsRequired();
            builder.Property(a => a.VIN).IsRequired();

            builder.HasOne(a => a.CarInfo)
                   .WithMany(c => c.Appointments)
                   .HasForeignKey(a => a.VIN);

            builder.HasOne(a => a.TypeAppointment)
                   .WithMany(ta => ta.Appointments)
                   .HasForeignKey(a => a.TypeAppointmentId);

            builder.HasOne(a => a.Customer)
                   .WithMany(c => c.Appointments)
                   .HasForeignKey(a => a.CustomerId)
                   .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
