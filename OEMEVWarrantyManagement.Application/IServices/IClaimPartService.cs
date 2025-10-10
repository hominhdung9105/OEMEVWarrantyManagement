using OEMEVWarrantyManagement.Application.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OEMEVWarrantyManagement.Application.IServices
{
    public interface IClaimPartService
    {
        Task<RequestClaimPart> CreateClaimPartAsync(RequestClaimPart dto);
        Task<bool> UpdateStatusClaimPartAsync(Guid claimPartId);
        Task<bool> CheckQuantityClaimPartAsync(Guid claimId);
        Task<IEnumerable<RequestClaimPart>> GetClaimPartsAsync(Guid claimId);
        //Task UpdateEnoughClaimPartsAsync();
    }
}
