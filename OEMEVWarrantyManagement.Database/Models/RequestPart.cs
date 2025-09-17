namespace OEMEVWarrantyManagement.Database.Models
{
    public class RequestPart
    {
        public string Id { get; set; }
        public string PartTypeId { get; set; } //FK
        public string status { get; set; }
        public string SCStaffId { get; set; } //FK
        public Employee SCStaff { get; set; } // Navigation property
        public string EVMStaffId { get; set; } //FK
        public Employee EVMStaff { get; set; } // Navigation property
        public ICollection<PartsRequestPart> PartsRequestParts { get; set; } = new List<PartsRequestPart>();
    }
}
