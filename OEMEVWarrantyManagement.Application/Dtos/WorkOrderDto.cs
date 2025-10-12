namespace OEMEVWarrantyManagement.Application.Dtos
{
    public class WorkOrderDto
    {
        public Guid? WorkOrderId { get; set; }
        public Guid? AssignedTo { get; set; }
        public string? Type { get; set; }
        public string? Target { get; set; }
        public Guid? TargetId { get; set; }
        public string? Status { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string? Notes { get; set; }
    }
    public class RequestCreateWorkOrderDto
    {
        public Guid? WorkOrderId { get; set; }
        public Guid? AssignedTo { get; set; }
        public string? Type { get; set; } // kiểm tra/ sửa chữa / thay thế
        public string? Target { get; set; }// claim hay campaign
        public Guid? TargetId { get; set; }
        public string? Status { get; set; }
        public DateTime? StartDate { get; set; }
    }

}
