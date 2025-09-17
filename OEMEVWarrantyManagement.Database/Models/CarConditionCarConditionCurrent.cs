namespace OEMEVWarrantyManagement.Database.Models
{
    public class CarConditionCarConditionCurrent
    {
        public int CarConditionId { get; set; } //FK
        public CarCondition CarCondition { get; set; }// Navigation property
        public int CarConditionCurrentId { get; set; }//FK
        public CarConditionCurrent CarConditionCurrent { get; set; }// Navigation property
    }
}
