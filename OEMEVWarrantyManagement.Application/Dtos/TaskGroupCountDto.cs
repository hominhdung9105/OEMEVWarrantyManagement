namespace OEMEVWarrantyManagement.Application.Dtos
{
    public class TaskGroupCountItemDto
    {
        public string Target { get; set; } // Warranty | Campaign
        public string Type { get; set; }   // Inspection | Repair
        public int Count { get; set; }
    }

    public class TaskGroupCountDto
    {
        public string Period { get; set; } // yyyy-MM for month, yyyy for year
        public int Total { get; set; }
        public IEnumerable<TaskGroupCountItemDto> Items { get; set; }
    }
}
