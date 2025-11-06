using Microsoft.EntityFrameworkCore;
using OEMEVWarrantyManagement.Domain.Entities;

namespace OEMEVWarrantyManagement.Infrastructure.Persistence;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }
    public DbSet<Organization> Organizations { get; set; }
    public DbSet<Employee> Employees { get; set; }
    public DbSet<Customer> Customers { get; set; }
    public DbSet<Vehicle> Vehicles { get; set; }
    public DbSet<Part> Parts { get; set; }
    public DbSet<VehiclePart> VehicleParts { get; set; }
    public DbSet<WarrantyPolicy> WarrantyPolicies { get; set; }
    public DbSet<VehicleWarrantyPolicy> VehicleWarrantyPolicies { get; set; }
    public DbSet<WarrantyClaim> WarrantyClaims { get; set; }
    public DbSet<ClaimAttachment> ClaimAttachments { get; set; }
    public DbSet<ClaimPart> ClaimParts { get; set; }
    public DbSet<WorkOrder> WorkOrders { get; set; }
    public DbSet<Campaign> Campaigns { get; set; }
    public DbSet<CampaignVehicle> CampaignVehicles { get; set; }
    public DbSet<PartOrder> PartOrders { get; set; }
    public DbSet<PartOrderItem> PartOrderItems { get; set; }
    public DbSet<BackWarrantyClaim> BackWarrantyClaims { get; set; }
    public DbSet<Appointment> Appointments { get; set; }
    public DbSet<CampaignVehicleReplacement> CampaignVehicleReplacements { get; set; }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // apply tất cả configurations trong assembly
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
    }
}
