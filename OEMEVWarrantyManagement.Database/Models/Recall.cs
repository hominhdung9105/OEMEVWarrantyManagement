namespace OEMEVWarrantyManagement.Database.Models
{
    public class Recall
    {
        public string Id { get; set; }
        public string EVMStaffId { get; set; }
        public string Name { get; set; }
        public string Status { get; set; }
        public string Detail { get; set; }
        public string PartReplacementId { get; set; }
        public int NumberOfCars { get; set; }
        
        public ICollection<RecallPartsReplacement> RecallPartsReplacements { get; set; } = new List<RecallPartsReplacement>();
        public ICollection<RecallHistory> RecallHistories { get; set; } = new List<RecallHistory>();

    }
}
