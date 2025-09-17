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
    public DbSet<CarCondition> CarConditions { get; set; }
    public DbSet<CarConditionCurrent> CarConditionCurrents { get; set; }
    public DbSet<CarConditionCarConditionCurrent> CarConditionCarConditionCurrents { get; set; }
    public DbSet<Image> Images { get; set; }
    public DbSet<WarrantyRequest> WarrantyRequests { get; set; }
    public DbSet<Assignment> Assignments { get; set; }
    public DbSet<Task> Tasks { get; set; }
    public DbSet<RequestPart> RequestParts { get; set; }
    public DbSet<Parts> Parts { get; set; }
    public DbSet<PartsRequestPart> PartsRequestParts { get; set; } //N-N relationship with Parts via PartsRequestPart
    public DbSet<DeliveryPart> DeliveryParts { get; set; }
    public DbSet<PartsDeliveryPart> PartsDeliveryParts { get; set; }
    public DbSet<WorkPlaceDeliveryPart> WorkPlaceDeliveryParts { get; set; }
    public DbSet<RoleEmployee> RoleEmployees { get; set; } //N-N relationship with Role via RoleEmployee
    public DbSet<WorkPlacePartTypeModel> WorkPlacePartTypeModels { get; set; } //N-N relationship with WorkPlaces via WorkPlacePartTypeModel
    public DbSet<PartReplaced> PartReplaceds { get; set; }
    public DbSet<Warranty> Warrantys { get; set; }
    public DbSet<Techs> Techs { get; set; }
    public DbSet<Customer> Customers { get; set; }
    public DbSet<PartsReplacement> PartsReplacements { get; set; }
    public DbSet<WarrantyPartsReplacement> WarrantyPartsReplacements { get; set; } 
    //N-N relationship with PartsInWarranty via WarrantyPartsInWarranty
    public DbSet<RecallHistory> RecallHistories { get; set; }
    public DbSet<RecallHistoryPartsReplacement> RecallHistoryPartsReplacements { get; set; }
    //N-N relationship with PartsReplacement via RecallHistoryPartsReplacement
    public DbSet<Recall> Recalls { get; set; }
    public DbSet<RecallPartsReplacement> RecallPartsReplacements { get; set; }



    //N-N relationship with DeliveryPart via DeliveryPartRequestPart


    //N-N relationship with CarConditionCurrent via CarConditionCarConditionCurrent

    //public DbSet<AppointmentTypeAppointment> AppointmentTypeAppointments { get; set; } //N-N relationship with TypeAppointment via AppointmentTypeAppointment
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // apply tất cả configurations trong assembly
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
    }
}
