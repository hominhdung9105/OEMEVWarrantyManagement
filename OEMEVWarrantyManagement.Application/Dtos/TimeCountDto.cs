using System;

namespace OEMEVWarrantyManagement.Application.Dtos
{
    public class TimeCountDto
    {
        public string Period { get; set; } = string.Empty; // e.g., 2024-11, 2024-11-06, 2024
        public int Count { get; set; }
    }
}
