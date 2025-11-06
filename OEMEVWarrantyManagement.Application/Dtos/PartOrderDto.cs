namespace OEMEVWarrantyManagement.Application.Dtos
{
    public class PartOrderDto
    {
        public Guid OrderId { get; set; }
        public Guid ServiceCenterId { get; set; }
        public DateTime RequestDate { get; set; }
        public DateTime? ApprovedDate { get; set; }
        public DateTime? ShippedDate { get; set; }
        public string Status { get; set; }
        public Guid CreatedBy { get; set; }
    }

    public class RequestPartOrderDto
    {
        public Guid ServiceCenterId { get; set; }
        public DateTime? RequestDate { get; set; }
        public Guid CreatedBy { get; set; }
        public string Status { get; set; }
    }

    public class ResponsePartOrderDto
    {
        public Guid OrderId { get; set; }
        public Guid ServiceCenterId { get; set; }
        public string? ServiceCenterName { get; set; }
        public DateTime RequestDate { get; set; }
        public DateTime? ApprovedDate { get; set; }
        public DateTime? ShippedDate { get; set; }
        public string? Status { get; set; }
        public Guid CreatedBy { get; set; }
        public string? CreatedByName { get; set; }
        public int TotalItems { get; set; }
        public DateOnly? ExpectedDate { get; set; }
        public DateTime? PartDelivery { get; set; }
        public List<ResponsePartOrderItemDto> PartOrderItems { get; set; }
    }

    public class ResponsePartOrderForScStaffDto
    {
        public Guid OrderId { get; set; }
        public Guid ServiceCenterId { get; set; }
        public string ServiceCenterName { get; set; }
        public DateTime RequestDate { get; set; }
        public DateTime? ApprovedDate { get; set; }
        public DateTime? ShippedDate { get; set; }
        public string Status { get; set; }
        public Guid CreatedBy { get; set; }
        public int TotalItems { get; set; }
        public DateOnly? ExpectedDate { get; set; }
        public DateTime? PartDelivery { get; set; }
        public List<ResponsePartOrderItemForScStaffDto> PartOrderItems { get; set; }
    }
}
