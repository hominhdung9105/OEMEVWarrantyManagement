using OEMEVWarrantyManagement.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OEMEVWarrantyManagement.Application.IRepository
{
    public interface IOrganizationRepository
    {
        Task<Organization> GetOrganizationById(Guid orgId);
        Task<IEnumerable<Organization>> GetAllOrganizationsAsync();
    }
}
