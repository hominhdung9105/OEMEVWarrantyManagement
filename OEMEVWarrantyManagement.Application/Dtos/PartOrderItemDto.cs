using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OEMEVWarrantyManagement.Application.Dtos
{
    public class PartOrderItemDto
    {
        public Guid OrderItemId { get; set; }
        public Guid OrderId { get; set; }
        public Guid PartId { get; set; }
        public int Quantity { get; set; }
        public string Remarks { get; set; }
    }

    public class RequsetPartOrderItemDto
    {
        public Guid OrderId { get; set; }
        public Guid PartId { get; set; }
        public int Quantity { get; set; }
        public string Remarks { get; set; }
    }
}
