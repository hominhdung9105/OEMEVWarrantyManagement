namespace OEMEVWarrantyManagement.Domain.Entities
{
    public class PartType
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public ICollection<PartTypeModel> PartTypeModels { get; set; } = new List<PartTypeModel>();
       
    }
}
