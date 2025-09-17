//using Microsoft.EntityFrameworkCore;
//using Microsoft.EntityFrameworkCore.Metadata.Builders;

//namespace OEMEVWarrantyManagement.Database.Models.Configurations
//{
//    public class AppointmentTypeAppointmentConfiguration : IEntityTypeConfiguration<AppointmentTypeAppointment>
//    {
//        public void Configure(EntityTypeBuilder<AppointmentTypeAppointment> builder)
//        {
//            builder.ToTable("AppointmentTypeAppointments")
//                   .HasKey(ata => new { ata.AppointmentId, ata.TypeAppointmentId });

//            builder.HasOne(ata => ata.Appointment)
//                   .WithMany(a => a.AppointmentsTypeAppointments)
//                   .HasForeignKey(ata => ata.AppointmentId);

//            builder.HasOne(ata => ata.TypeAppointment)
//                   .WithMany(ta => ta.AppointmentsTypeAppointments)
//                   .HasForeignKey(ata => ata.TypeAppointmentId);   
//        }
//    }
//}
//N-N relationship with TypeAppointment via AppointmentTypeAppointment
