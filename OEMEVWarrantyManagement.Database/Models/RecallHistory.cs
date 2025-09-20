namespace OEMEVWarrantyManagement.Database.Models
{
    public class RecallHistory
    {
        public string Id { get; set; }
        public string VIN { get; set; }//FK
        public CarInfo CarInfo { get; set; } //Navigation property
        public string Status { get; set; }
        public string PartReplacementId { get; set; }
        public string RecallId { get; set; } //FK
        public Recall Recall { get; set; } //Navigation property
        public string EmpoloyeeSCStaffId { get; set; } //FK
        public Employee EmployeeStaffs { get; set; } //Navigation property
        public DateTime DateStart { get; set; }
        public DateTime DateEnd { get; set; }
        public ICollection<PartsReplacement> PartsReplacements { get; set; }
        public ICollection<RecallHistoryEmployee> RecallHistoryEmployees { get; set; } = new List<RecallHistoryEmployee>();
    }

}
