using System;

namespace OEMEVWarrantyManagement.Application.Dtos
{
    public class AssignedTechDto
    {
        public Guid UserId { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
    }
}
