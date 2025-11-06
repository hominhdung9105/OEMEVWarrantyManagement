using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OEMEVWarrantyManagement.Application.Dtos
{
    public class PartDto
    {
        public string Model { get; set; }
        public string Name { get; set; }
        public string Category { get; set; }
        public int StockQuantity { get; set; }
        public string Status { get; set; }
        //public Guid OrgId { get; set; }
    }

}
