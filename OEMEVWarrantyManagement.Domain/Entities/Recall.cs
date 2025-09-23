namespace OEMEVWarrantyManagement.Domain.Entities
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
        
        public ICollection<RecallHistory> RecallHistories { get; set; } = new List<RecallHistory>();

        public ICollection<RecallPartTypeModel> RecallPartTypeModels { get; set; }

    }
}
