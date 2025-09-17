namespace OEMEVWarrantyManagement.Database.Models
{
    public class WorkPlaces
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Location { get; set; }
        public ICollection<Employee> Employees { get; set; } = new List<Employee>();
        public ICollection<WorkPlaceDeliveryPart> WorkPlaceDeliveryParts { get; set; } = new List<WorkPlaceDeliveryPart>();
        public ICollection<WorkPlacePartTypeModel> WorkPlacePartTypeModels { get; set; } = new List<WorkPlacePartTypeModel>();


    }
}
