using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace OEMEVWarrantyManagement.Database.Models.Configurations
{
    public class TypeAppointmentConfiguration : IEntityTypeConfiguration<TypeAppointment>
    {
        public void Configure(EntityTypeBuilder<TypeAppointment> builder)
        {   
            builder.ToTable("TypeAppointments");
            builder.HasKey(ta => ta.Id);
            builder.Property(ta => ta.Name)
                   .IsRequired()
                   .HasMaxLength(100);

        }
    }
}
