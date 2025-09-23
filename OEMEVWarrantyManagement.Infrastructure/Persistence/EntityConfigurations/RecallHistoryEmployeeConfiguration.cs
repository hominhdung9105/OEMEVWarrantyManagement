using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OEMEVWarrantyManagement.Domain.Entities;

namespace OEMEVWarrantyManagement.Infrastructure.Persistence.EntityConfigurations
{
    internal class RecallHistoryEmployeeConfiguration : IEntityTypeConfiguration<RecallHistoryEmployee>
    {
        public void Configure(EntityTypeBuilder<RecallHistoryEmployee> builder)
        {
            builder.HasKey(rht => new { rht.RecallHistoryId, rht.EmployeeId });

            builder.HasOne(rht => rht.RecallHistory)
                   .WithMany(rh => rh.RecallHistoryEmployees)
                   .HasForeignKey(rht => rht.RecallHistoryId);

            builder.HasOne(rht => rht.Employee)
                   .WithMany(e => e.RecallHistoryEmployees) 
                   .HasForeignKey(rht => rht.EmployeeId)
                   .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
