namespace OEMEVWarrantyManagement.Database.Models
{
    public class WorkPlaces
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Location { get; set; }
        public ICollection<Employee> Employees { get; set; } = new List<Employee>();
        public ICollection<WorkPlacePartTypeModel> WorkPlacePartTypeModels { get; set; } = new List<WorkPlacePartTypeModel>();
        public ICollection<DeliveryPart> DeliveryParts { get; set; } = new List<DeliveryPart>();


    }
}
