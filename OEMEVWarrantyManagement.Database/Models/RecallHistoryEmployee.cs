using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OEMEVWarrantyManagement.Database.Models
{
    public class RecallHistoryEmployee
    {
        public string RecallHistoryId { get; set; }
        public RecallHistory RecallHistory { get; set; }

        public string EmployeeId { get; set; }
        public Employee Employee { get; set; }
    }
}
