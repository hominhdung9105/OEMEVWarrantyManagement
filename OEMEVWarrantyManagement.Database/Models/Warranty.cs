namespace OEMEVWarrantyManagement.Database.Models
{
    public class Warranty
    {
        public string Id { get; set; }
        public string EmployeeTechId { get; set; } //FK
        public Techs EmployeeTech { get; set; } // Navigation property
        public string Status { get; set; }
        public string RequestWarrantyId { get; set; } //FK
        public WarrantyRequest RequestWarranty { get; set; } // Navigation property
        public string PartRereplacementId { get; set; } //FK
        public string WarrantyRecordId { get; set; } //FK
        public WarrantyRecord WarrantyRecord { get; set; } // Navigation property
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string EmployeeSCStaffId { get; set; } //FK
        public Employee EmployeeSCStaff { get; set; } // Navigation property
        public ICollection<WarrantyPartsReplacement> WarrantyPartReplacements { get; set; } = new List<WarrantyPartsReplacement>();

    }
}
