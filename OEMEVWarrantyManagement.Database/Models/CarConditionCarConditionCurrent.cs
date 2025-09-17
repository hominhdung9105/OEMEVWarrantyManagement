namespace OEMEVWarrantyManagement.Database.Models
{
    public class CarConditionCarConditionCurrent
    {
        public string CarConditionId { get; set; } //FK
        public CarCondition CarCondition { get; set; }// Navigation property
        public string CarConditionCurrentId { get; set; }//FK
        public CarConditionCurrent CarConditionCurrent { get; set; }// Navigation property
    }
}
