namespace OEMEVWarrantyManagement.Database.Models
{
    public class PartTypeModel
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string PartTypeId { get; set; }//FK
        public PartType PartType { get; set; }//Navigation property
        public ICollection<CarCondition> CarConditions { get; set; } = new List<CarCondition>();
        public ICollection<Parts> Parts { get; set; } = new List<Parts>();
        public ICollection<WorkPlacePartTypeModel> WorkPlacePartTypeModels { get; set; } = new List<WorkPlacePartTypeModel>();
        public ICollection<PartReplaced> PartReplaceds { get; set; } = new List<PartReplaced>();
        public ICollection<PartsReplacement> PartsReplacements { get; set; } = new List<PartsReplacement>();
    }
}
