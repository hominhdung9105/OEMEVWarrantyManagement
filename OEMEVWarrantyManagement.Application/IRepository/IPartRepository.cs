using OEMEVWarrantyManagement.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OEMEVWarrantyManagement.Application.IRepository
{
    public interface IPartRepository
    {
        Task<IEnumerable<Part>> GetAllAsync();
        Task<IEnumerable<Part>> GetByOrgIdAsync(Guid orgId);
        Task<IEnumerable<Part>> GetPartsAsync(string model = null, string category = null);
        Task<Part> GetPartsByIdAsync(Guid PartId); 
    }
}
