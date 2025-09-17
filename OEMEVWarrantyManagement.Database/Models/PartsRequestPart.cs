namespace OEMEVWarrantyManagement.Database.Models
{
    public class PartsRequestPart
    {
        public string Id { get; set; }
        public string RequestPartId { get; set; } //FK
        public RequestPart RequestPart { get; set; } // Navigation property
        public string PartsId { get; set; } //FK
        public Parts Parts { get; set; } // Navigation property
    }
}
