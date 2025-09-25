using OEMEVWarrantyManagement.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OEMEVWarrantyManagement.Application.Dtos
{
    public class WarrantyRequestDto
    {
        public Guid Id { get; set; }
        public string? VIN { get; set; }
        public Guid? SCStaffId { get; set; }
        public string Status { get; set; }
        public Guid? EVMStaffId { get; set; }
        public Guid? CarConditionCurrentId { get; set; }
        public DateTime? RequestDate { get; set; }
        public DateTime? ResponseDate { get; set; }
    }
}
