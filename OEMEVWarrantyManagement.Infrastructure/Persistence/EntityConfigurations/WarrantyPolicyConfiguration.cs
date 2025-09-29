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
    public class WarrantyPolicyConfiguration : IEntityTypeConfiguration<WarrantyPolicy>
    {
        public void Configure(EntityTypeBuilder<WarrantyPolicy> builder)
        {
            builder.ToTable("WarrantyPolicies");
            builder.HasKey(p => p.PolicyId);
            builder.Property(p => p.PolicyId).ValueGeneratedOnAdd();
            builder.Property(p => p.Name).IsRequired();
            builder.Property(p => p.CoveragePeriodMonths);
            builder.Property(p => p.Conditions);
            //builder.Property(p => p.OrgId);

            //builder.HasOne(p => p.Organization)
            //       .WithMany(o => o.WarrantyPolicies)
            //       .HasForeignKey(p => p.OrgId)
            //       .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
