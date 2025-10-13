using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OEMEVWarrantyManagement.Application.Dtos
{
    public class VehicleWarrantyPolicyDto
    {
        public Guid VehicleWarrantyId { get; set; }
        public string Vin { get; set; }
        public Guid PolicyId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string Status { get; set; }
    }

    public class PolicyInformationDto
    {
        public string PolicyName { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
    }
}
