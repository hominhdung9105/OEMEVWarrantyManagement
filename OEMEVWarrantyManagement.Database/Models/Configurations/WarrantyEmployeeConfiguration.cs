using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OEMEVWarrantyManagement.Database.Models.Configurations
{
    public class WarrantyEmployeeConfiguration : IEntityTypeConfiguration<WarrantyEmployee>

    {
        public void Configure(EntityTypeBuilder<WarrantyEmployee> builder)
        {
            builder.HasKey(wt => new { wt.WarrantyId, wt.EmployeeId });
            builder.HasOne(wt => wt.Warranty)
                   .WithMany(w => w.WarrantyEmployees)
                   .HasForeignKey(wt => wt.WarrantyId);

            builder.HasOne(wt => wt.Employee)
                   .WithMany(e => e.WarrantyEmployees)
                   .HasForeignKey(wt => wt.EmployeeId)
                   .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
