namespace OEMEVWarrantyManagement.Database.Models
{
    public class WorkPlacePartTypeModel
    {
        public int WorkPlacesId { get; set; } //FK
        public WorkPlaces WorkPlaces { get; set; } // Navigation property
        public int PartTypeModelId { get; set; } //FK
        public PartTypeModel PartTypeModel { get; set; } // Navigation property
        public int Number { get; set; }
    }
}
