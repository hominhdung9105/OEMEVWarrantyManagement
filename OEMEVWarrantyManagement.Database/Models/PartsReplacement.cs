using System.ComponentModel.DataAnnotations;

namespace OEMEVWarrantyManagement.Database.Models
{
    public class PartsReplacement
    {
        public string SerialNumber { get; set; }
        public string PartTypeModelId { get; set; }//FK
        public PartTypeModel PartTypeModel { get; set; }//Navigation property

        public string? WarrantyId { get; set; }//FK
        public   Warranty Warranty { get; set; }//Navigation property

        public string? RecallHistoryId { get; set; }//FK
        public  RecallHistory RecallHistory { get; set; }//Navigation property
        public ICollection<WarrantyPartsReplacement> WarrantyPartReplacements { get; set; } = new List<WarrantyPartsReplacement>();
    }
}
