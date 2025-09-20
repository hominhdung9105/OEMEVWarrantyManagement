using System.Data;

namespace OEMEVWarrantyManagement.Database.Models
{
    public class Employee
    {
        public Guid Id { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string RoleId { get; set; }     //FK
        public Role Role { get; set; }      // Navigation property
        public string FullName { get; set; }

        public string? RefreshToken { get; set; }
        public DateTime? RefreshTokenExpiryTime { get; set; }

        public string WorkPlacesId { get; set; } //FK
        public WorkPlaces WorkPlaces { get; set; } // Navigation property
        public ICollection<CarConditionCurrent> CarConditionCurrents { get; set; } = new List<CarConditionCurrent>();
        public ICollection<Assignment> AssignmentsAsSCStaff { get; set; } = new List<Assignment>();
        public ICollection<Assignment> AssignmentsAsSCTech { get; set; } = new List<Assignment>();
        public ICollection<RequestPart> RequestParts { get; set; } = new List<RequestPart>();
        //public ICollection<RoleEmployee> RoleEmployees { get; set; } = new List<RoleEmployee>();
        public ICollection<DeliveryPart> DeliveryPartsSend { get; set; } = new List<DeliveryPart>();
        public ICollection<DeliveryPart> DeliveryPartsReceive { get; set; } = new List<DeliveryPart>();
        public ICollection<WarrantyRequest> WarrantyRequestsAsEVMStaff { get; set; } = new List<WarrantyRequest>();
        public ICollection<WarrantyRequest> WarrantyRequestsAsSCStaff { get; set; } = new List<WarrantyRequest>();
        public ICollection<Warranty> WarrantiesAsSCTech { get; set; } = new List<Warranty>();
        public ICollection<Customer> Customer { get; set; } = new List<Customer>();
        public ICollection<RecallHistory> RecallHistoriesAsSCStaff { get; set; } = new List<RecallHistory>();

        public ICollection<WarrantyEmployee> WarrantyEmployees { get; set; } = new List<WarrantyEmployee>();

        public ICollection<RecallHistoryEmployee> RecallHistoryEmployees { get; set; } = new List<RecallHistoryEmployee>();

    }
}
