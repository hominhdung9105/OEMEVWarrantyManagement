using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OEMEVWarrantyManagement.Application.Dtos
{
    public class WarrantyPolicyDto
    {
        public Guid PolicyId { get; set; }
        public string Name { get; set; }
        public int CoveragePeriodMonths { get; set; }
        public string Conditions { get; set; }
    }

}
