using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OEMEVWarrantyManagement.Domain.Entities
{
    public class ClaimAttachment
    {
        public int AttachmentId { get; set; }
        public int ClaimId { get; set; }
        public string URL { get; set; }
        public int UploadedBy { get; set; }

        // Navigation Properties
        public WarrantyClaim WarrantyClaim { get; set; }
        public Employee UploadedByEmployee { get; set; }
    }
}
