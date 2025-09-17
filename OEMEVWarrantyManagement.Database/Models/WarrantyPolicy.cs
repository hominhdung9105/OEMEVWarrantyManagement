namespace OEMEVWarrantyManagement.Database.Models
{
    public class WarrantyPolicy
    {
        public string Id { get; set; }
        public string PeriodInMonths { get; set; }
        public string Coverage { get; set; }
        public string Conditions { get; set; }

        public ICollection<WarrantyRecord> WarrantyRecords { get; set; } = new List<WarrantyRecord>();
    }
}
