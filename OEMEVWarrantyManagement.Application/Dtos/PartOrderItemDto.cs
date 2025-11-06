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
        public string Model { get; set; }
        public int Quantity { get; set; }
        public string Remarks { get; set; }
    }

    public class RequsetPartOrderItemDto
    {
        public Guid? OrderId { get; set; }
        public string Model { get; set; }
        public int Quantity { get; set; }
        public string Remarks { get; set; }
    }

    public class ResponsePartOrderItemDto
    {
        public Guid OrderId { get; set; }
        public string? Name { get; set; }
        public int ScStock { get; set; }
        public int OemStock { get; set; }
        public string? Model { get; set; }
        public int Quantity { get; set; }
        public string? Remarks { get; set; }
    }
    public class ResponsePartOrderItemForScStaffDto
    {
        public Guid OrderItemId { get; set; }
        public Guid OrderId { get; set; }
        public int RequestedQuantity { get; set; }
        public string Model { get; set; }
        public int ScStock { get; set; }
    }
    public class UpdateExpectedDateDto
    {
        public Guid? OrderId { get; set; }
        public DateOnly? ExpectedDate { get; set; }
    }
}
