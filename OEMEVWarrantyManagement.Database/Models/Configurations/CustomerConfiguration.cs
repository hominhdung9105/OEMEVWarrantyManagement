using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace OEMEVWarrantyManagement.Database.Models.Configurations
{
    public class CustomerConfiguration : IEntityTypeConfiguration<Customer>
    {
        public void Configure(EntityTypeBuilder<Customer> builder)
        {
            builder.ToTable("Customers");
            builder.HasKey(c => c.Id);
            builder.Property(c => c.FullName)
                   .IsRequired()
                   .HasMaxLength(100);
            builder.Property(c => c.PhoneNumber)
                   .IsRequired()
                   .HasMaxLength(15);
            builder.Property(c => c.Email)
                   .IsRequired();
            builder.Property(c => c.EmployeeId)
                   .IsRequired();

            builder.HasOne(c => c.Employee)
                   .WithMany(e => e.Customer)
                   .HasForeignKey(c => c.EmployeeId);
                    
        }
    }
}
