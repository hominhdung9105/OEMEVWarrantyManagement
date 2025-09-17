namespace OEMEVWarrantyManagement.Database.Models
{
    public class CarCondition
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int PartTypeModelId { get; set; } // Foreign Key
        public PartTypeModel PartTypeModel { get; set; } // Navigation Property
        public ICollection<CarConditionCarConditionCurrent> CarConditionCarConditionCurrents { get; set; } = new List<CarConditionCarConditionCurrent>();
    }
}
