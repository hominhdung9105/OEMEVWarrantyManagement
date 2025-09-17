namespace OEMEVWarrantyManagement.Database.Models
{
    public class RequestPart
    {
        public int Id { get; set; }
        public int PartTypeId { get; set; } //FK
        public string status { get; set; }
        public int SCStaffId { get; set; } //FK
        public Employee SCStaff { get; set; } // Navigation property
        public int EVMStaffId { get; set; } //FK
        public Employee EVMStaff { get; set; } // Navigation property
        public ICollection<PartsRequestPart> PartsRequestParts { get; set; } = new List<PartsRequestPart>();
    }
}
