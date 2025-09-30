using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OEMEVWarrantyManagement.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OEMEVWarrantyManagement.Infrastructure.Persistence.EntityConfigurations
{
    public class CustomerConfiguration : IEntityTypeConfiguration<Customer>
    {
        public void Configure(EntityTypeBuilder<Customer> builder)
        {
            builder.ToTable("Customers");
            builder.HasKey(c => c.CustomerId);
            builder.Property(c => c.CustomerId).ValueGeneratedOnAdd();
            builder.Property(c => c.Name).IsRequired();
            builder.Property(c => c.Phone);
            builder.Property(c => c.Email);
            builder.Property(c => c.Address);
            //builder.Property(c => c.OrgId);

            //builder.HasOne(c => c.Organization)
            //       .WithMany(o => o.Customers)
            //       .HasForeignKey(c => c.OrgId)
            //       .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
