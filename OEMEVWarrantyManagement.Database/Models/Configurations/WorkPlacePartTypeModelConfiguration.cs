using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace OEMEVWarrantyManagement.Database.Models.Configurations
{
    public class WorkPlacePartTypeModelConfiguration : IEntityTypeConfiguration<WorkPlacePartTypeModel>
    {
        public void Configure(EntityTypeBuilder<WorkPlacePartTypeModel> builder)
        {
            builder.ToTable("WorkPlacePartTypeModels(PartInStock)");
            builder.HasKey(wptm => new { wptm.WorkPlacesId, wptm.PartTypeModelId });
            builder.Property(wptm => wptm.Number).IsRequired();

            // relationships with WorkPlaces and PartTypeModel
            builder.HasOne(wptm => wptm.WorkPlaces)
                   .WithMany(wp => wp.WorkPlacePartTypeModels)
                   .HasForeignKey(wptm => wptm.WorkPlacesId)
                   .OnDelete(DeleteBehavior.Cascade);
            // Relationship with PartTypeModel
            builder.HasOne(wptm => wptm.PartTypeModel)
                   .WithMany(ptm => ptm.WorkPlacePartTypeModels)
                   .HasForeignKey(wptm => wptm.PartTypeModelId)
                   .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
