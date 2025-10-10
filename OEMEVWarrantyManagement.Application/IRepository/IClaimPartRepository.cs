using OEMEVWarrantyManagement.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OEMEVWarrantyManagement.Application.IRepository
{
    public interface IClaimPartRepository
    {
        Task<ClaimPart> CreateClaimPartAsync(ClaimPart request);
        Task<IEnumerable<ClaimPart>> GetClaimPartByClaimIdAsync(Guid claimId);
        Task UpdateRangeAsync(IEnumerable<ClaimPart> entities);
        Task<IEnumerable<ClaimPart>> GetAllNotEnoughAsync();
    }
}
