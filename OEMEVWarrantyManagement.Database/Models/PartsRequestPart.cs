namespace OEMEVWarrantyManagement.Database.Models
{
    public class PartsRequestPart
    {
        public int Id { get; set; }
        public int RequestPartId { get; set; } //FK
        public RequestPart RequestPart { get; set; } // Navigation property
        public int PartsId { get; set; } //FK
        public Parts Parts { get; set; } // Navigation property
    }
}
