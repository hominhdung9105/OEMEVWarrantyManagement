using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OEMEVWarrantyManagement.Application.Dtos
{
    public class BackWarrantyClaimDto
    {
        public string Description { get; set; }
        public DateTime CreatedDate { get; set; }
        public Guid WarrantyClaimId { get; set; }
        public Guid CreatedByEmployeeId { get; set; }

    }

    public class CreateBackWarrantyClaimRequestDto
    {
        public Guid? WarrantyClaimId { get; set; }
        public string Description { get; set; }
    }
}
