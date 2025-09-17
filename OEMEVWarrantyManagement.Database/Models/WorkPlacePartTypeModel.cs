namespace OEMEVWarrantyManagement.Database.Models
{
    public class WorkPlacePartTypeModel
    {
        public string WorkPlacesId { get; set; } //FK
        public WorkPlaces WorkPlaces { get; set; } // Navigation property
        public string PartTypeModelId { get; set; } //FK
        public PartTypeModel PartTypeModel { get; set; } // Navigation property
        public int Number { get; set; }
    }
}
