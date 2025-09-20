namespace OEMEVWarrantyManagement.Database.Models
{
    public class Warranty
    {
        public Guid Id { get; set; }
        public string Status { get; set; }
        public Guid RequestWarrantyId { get; set; } //FK
        public WarrantyRequest RequestWarranty { get; set; } // Navigation property
        public Guid WarrantyRecordId { get; set; } //FK
        public WarrantyRecord WarrantyRecord { get; set; } // Navigation property
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public Guid EmployeeSCStaffId { get; set; } //FK
        public Employee EmployeeSCStaff { get; set; } // Navigation property
        public ICollection<WarrantyPartsReplacement> WarrantyPartReplacements { get; set; } = new List<WarrantyPartsReplacement>();

        public ICollection<PartsReplacement> PartsReplacements { get; set; }

        public ICollection<WarrantyEmployee> WarrantyEmployees { get; set; } = new List<WarrantyEmployee>();

    }
}
