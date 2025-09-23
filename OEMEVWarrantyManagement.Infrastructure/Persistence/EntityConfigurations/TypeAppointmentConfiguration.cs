using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OEMEVWarrantyManagement.Domain.Entities;

namespace OEMEVWarrantyManagement.Infrastructure.Persistence.EntityConfigurations
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
