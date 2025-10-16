using OEMEVWarrantyManagement.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OEMEVWarrantyManagement.Application.IRepository
{
    public interface IBackWarrantyClaimRepository
    {
        Task<IEnumerable <BackWarrantyClaim>> GetAllBackWarrantyClaimsAsync();
        Task<BackWarrantyClaim> CreateBackWarrantyClaimAsync(BackWarrantyClaim entity);
    }
}
