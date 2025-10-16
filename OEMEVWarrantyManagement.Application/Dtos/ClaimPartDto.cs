namespace OEMEVWarrantyManagement.Application.Dtos
{
    public class ClaimPartDto
    {
        public Guid ClaimPartId { get; set; }
        public Guid ClaimId { get; set; }
        public string Model { get; set; }
        public int Quantity { get; set; }
        public string Action { get; set; } // repair | replace
        public string Status { get; set; }
        public decimal Cost { get; set; } // TODO - chưa xử lí
    }

    public class RequestClaimPart
    {
        public Guid ClaimId { get; set; }
        public Guid PartId { get; set; }
        public string Model { get; set; }
        public int Quantity { get; set; }
        public string Action { get; set; }
        public string? Status { get; set; }
    }

    public class PartsInClaimPartDto
    {
        public Guid PartId { get; set; }
        public string Model { get; set; }
        //public int Quantity { get; set; }
        public string SerialNumber { get; set; }
        public string Action { get; set; }
        public string? Status { get; set; }
    }

    public class UpdateClaimPartDto
    {
        public Guid ClaimPartId { get; set; }
        public required string SerialNumber { get; set; }
    }
}
