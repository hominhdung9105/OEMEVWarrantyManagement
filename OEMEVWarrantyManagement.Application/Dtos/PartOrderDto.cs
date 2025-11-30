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

    /// <summary>
    /// DTO chi tiết đầy đủ cho Part Order bao gồm items, shipments, receipts, issues và resolution
    /// Dùng cho cả 3 role: SC Staff, EVM Staff, Admin
    /// </summary>
    public class ResponsePartOrderDetailDto
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
        
        // Order items with full detail
        public List<ResponsePartOrderItemDetailDto> PartOrderItems { get; set; } = new List<ResponsePartOrderItemDetailDto>();
        
        // Shipment information (for EVM/Admin - what was sent)
        public List<PartOrderShipmentDto>? Shipments { get; set; }
        
        // Receipt information (for SC/Admin - what was received)
        public List<PartOrderReceiptDto>? Receipts { get; set; }
        
        // Issues (cancellation, return)
        public List<PartOrderIssueDto>? Issues { get; set; }
        
        // Discrepancy resolution (for Admin)
        public DiscrepancyResolutionDto? DiscrepancyResolution { get; set; }
        
        // Summary statistics
        public PartOrderStatisticsDto? Statistics { get; set; }
    }

    /// <summary>
    /// DTO cho shipment information
    /// </summary>
    public class PartOrderShipmentDto
    {
        public Guid ShipmentId { get; set; }
        public Guid OrderId { get; set; }
        public string Model { get; set; }
        public string SerialNumber { get; set; }
        public DateTime ShippedAt { get; set; }
        public string Status { get; set; }
    }

    /// <summary>
    /// DTO cho receipt information
    /// </summary>
    public class PartOrderReceiptDto
    {
        public Guid ReceiptId { get; set; }
        public Guid OrderId { get; set; }
        public string Model { get; set; }
        public string SerialNumber { get; set; }
        public DateTime? ReceivedAt { get; set; }
        public string Status { get; set; }
        public string? Note { get; set; }
        public string? ImageUrl { get; set; }
    }

    /// <summary>
    /// DTO cho order item với đầy đủ thông tin stock
    /// </summary>
    public class ResponsePartOrderItemDetailDto
    {
        public Guid OrderItemId { get; set; }
        public Guid OrderId { get; set; }
        public string Model { get; set; }
        public string? Name { get; set; }
        public int RequestedQuantity { get; set; }
        public int? ShippedQuantity { get; set; }
        public int? ReceivedQuantity { get; set; }
        public int? DamagedQuantity { get; set; }
        public int ScStock { get; set; }
        public int? OemStock { get; set; }
        public string? Remarks { get; set; }
    }

    /// <summary>
    /// DTO cho thống kê tổng quan của order
    /// </summary>
    public class PartOrderStatisticsDto
    {
        public int TotalRequested { get; set; }
        public int TotalShipped { get; set; }
        public int TotalReceived { get; set; }
        public int TotalDamaged { get; set; }
        public int TotalMissing { get; set; }
        public bool HasDiscrepancy { get; set; }
    }
}
