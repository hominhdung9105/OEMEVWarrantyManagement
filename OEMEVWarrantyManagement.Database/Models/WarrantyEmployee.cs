using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OEMEVWarrantyManagement.Database.Models
{
    public class WarrantyEmployee
    {
        public Guid WarrantyId { get; set; }
        public Warranty Warranty { get; set; }

        public Guid EmployeeId { get; set; }
        public Employee Employee { get; set; }
    }
}
