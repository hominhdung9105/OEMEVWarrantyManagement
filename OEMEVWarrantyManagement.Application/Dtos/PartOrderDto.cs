using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OEMEVWarrantyManagement.Application.Dtos
{
    public class PartOrderDto
    {
        public Guid OrderId { get; set; }
        public Guid ServiceCenterId { get; set; }
        public DateTime RequestDate { get; set; }
        public DateTime? ApprovedDate { get; set; }
        public DateTime? ShippedDate { get; set; }
        public string Status { get; set; }
        public Guid CreatedBy { get; set; }
    }

    public class RequestPartOrderDto
    {
        public Guid ServiceCenterId { get; set; }
        public DateTime? RequestDate { get; set; }
        public Guid CreatedBy { get; set; }
        public string Status { get; set; }
    }
}
