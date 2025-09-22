namespace OEMEVWarrantyManagement.Database.Data;
using Microsoft.EntityFrameworkCore;
using OEMEVWarrantyManagement.Database.Models;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }
    public DbSet<Employee> Employees { get; set; }
    public DbSet<WorkPlaces> WorkPlaces { get; set; }
    public DbSet<Role> Roles { get; set; }
    public DbSet<WarrantyRecord> WarrantyRecords { get; set; }
    public DbSet<WarrantyPolicy> WarrantyPolicies { get; set; }
    public DbSet<CarInfo> CarInfos { get; set; }
    public DbSet<Appointment> Appointments { get; set; }
    public DbSet<TypeAppointment> TypeAppointments { get; set; }
    public DbSet<CarModel> CarModels { get; set; }
    public DbSet<PartType> PartTypes { get; set; }
    public DbSet<PartTypeModel> PartTypeModels { get; set; }
    public DbSet<CarConditionCurrent> CarConditionCurrents { get; set; }
    public DbSet<Image> Images { get; set; }
    public DbSet<WarrantyRequest> WarrantyRequests { get; set; }
    public DbSet<Assignment> Assignments { get; set; }
    public DbSet<RequestPart> RequestParts { get; set; }
    public DbSet<Parts> Parts { get; set; }
    public DbSet<DeliveryPart> DeliveryParts { get; set; }
    public DbSet<WorkPlacePartTypeModel> WorkPlacePartTypeModels { get; set; }
    public DbSet<Warranty> Warrantys { get; set; }
    public DbSet<Customer> Customers { get; set; }
    public DbSet<PartsReplacement> PartsReplacements { get; set; }
    public DbSet<WarrantyPartsReplacement> WarrantyPartsReplacements { get; set; } 
    public DbSet<RecallHistory> RecallHistories { get; set; }
    public DbSet<Recall> Recalls { get; set; }
    public DbSet<RecallPartTypeModel> RecallPartsReplacements { get; set; }
    public DbSet<WarrantyEmployee> WarrantyEmployees { get; set; }
    public DbSet<RecallHistoryEmployee> RecallHistoryEmployees { get; set; }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // apply tất cả configurations trong assembly
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
    }
}
