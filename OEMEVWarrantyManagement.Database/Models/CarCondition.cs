namespace OEMEVWarrantyManagement.Database.Models
{
    public class CarCondition
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string PartTypeModelId { get; set; } // Foreign Key
        public PartTypeModel PartTypeModel { get; set; } // Navigation Property
        public ICollection<CarConditionCarConditionCurrent> CarConditionCarConditionCurrents { get; set; } = new List<CarConditionCarConditionCurrent>();
    }
}
