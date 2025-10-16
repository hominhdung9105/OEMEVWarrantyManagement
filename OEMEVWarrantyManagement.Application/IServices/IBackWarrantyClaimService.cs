using OEMEVWarrantyManagement.Application.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OEMEVWarrantyManagement.Application.IServices
{
    public interface IBackWarrantyClaimService
    {
        Task<BackWarrantyClaimDto> CreateBackWarrantyClaimAsync(CreateBackWarrantyClaimRequestDto request);
        Task<IEnumerable<BackWarrantyClaimDto>> GetAllBackWarrantyClaimsAsync();
    }
}
