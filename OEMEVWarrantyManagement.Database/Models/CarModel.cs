namespace OEMEVWarrantyManagement.Database.Models
{
    public class CarModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public ICollection<CarInfo> CarInfos { get; set; } = new List<CarInfo>();
    }
}
