namespace OEMEVWarrantyManagement.Domain.Entities
{
    public class CarModel
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public ICollection<CarInfo> CarInfos { get; set; } = new List<CarInfo>();
    }
}
