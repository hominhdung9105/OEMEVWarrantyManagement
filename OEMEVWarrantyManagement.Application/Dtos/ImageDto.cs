namespace OEMEVWarrantyManagement.Application.Dtos
{
    public class ImageDto
    {
        public string AttachmentId { get; set; }
        public Guid ClaimId { get; set; }
        public string URL { get; set; }
        public Guid UploadedBy { get; set; }
    }
}
