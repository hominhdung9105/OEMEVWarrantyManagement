using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OEMEVWarrantyManagement.Domain.Entities
{
    public class BackWarrantyClaim
    {
        public string Description { get; set; }
        public DateTime CreatedDate { get; set; }
        public Guid WarrantyClaimId { get; set; }
        public WarrantyClaim WarrantyClaim { get; set; }

        public Guid CreatedByEmployeeId { get; set; }
        public Employee CreatedByEmployee { get; set; }
    }
}
