using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace OEMEVWarrantyManagement.Database.Models.Configurations
{
    public class AppointmentConfiguration : IEntityTypeConfiguration<Appointment>
    {
        public void Configure(EntityTypeBuilder<Appointment> builder)
        {
            builder.ToTable("Appointments");
            builder.HasKey(a => a.Id);
            builder.Property(a => a.EmployeeId).IsRequired();
            builder.Property(a => a.CustomerId).IsRequired();
            builder.Property(a => a.Note).HasMaxLength(500);
            builder.Property(a => a.Date).IsRequired();
            builder.Property(a => a.Time).IsRequired();
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
