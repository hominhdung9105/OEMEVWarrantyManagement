using AutoMapper;
using OEMEVWarrantyManagement.Application.Dtos;
using OEMEVWarrantyManagement.Application.IRepository;
using OEMEVWarrantyManagement.Application.IServices;
using OEMEVWarrantyManagement.Domain.Entities;

namespace OEMEVWarrantyManagement.Application.Services
{
    public class ClaimPartService : IClaimPartService
    {
        private readonly IClaimPartRepository _claimPartReposotory;
        private readonly IMapper _mapper;
        private readonly IPartRepository _partRepository;
        private readonly IWarrantyClaimRepository _warrantyClaimRepository;

        public ClaimPartService(IClaimPartRepository claimPartRepository, IMapper mapper, IPartRepository partRepository, IWarrantyClaimRepository warrantyClaimRepository)
        {
            _claimPartReposotory = claimPartRepository;
            _mapper = mapper;
            _partRepository = partRepository;
            _warrantyClaimRepository = warrantyClaimRepository;
        }

        public async Task<List<RequestClaimPart>> CreateManyClaimPartsAsync(InspectionDto dto)
        {
            var entities = dto.Parts.Select(p => new ClaimPart
            {
                ClaimId = (Guid)dto.ClaimId,
                Model = p.Model,
                Quantity = p.Quantity,
                Action = p.Action,
                Status = p.Status,
                Cost = 0
            }).ToList();

            await _claimPartReposotory.CreateManyClaimPartsAsync(entities);
            return _mapper.Map<List<RequestClaimPart>>(entities);
        }

        public async Task<IEnumerable<RequestClaimPart>> GetClaimPartsAsync(Guid claimId)
        {
            var entities = await _claimPartReposotory.GetClaimPartByClaimIdAsync(claimId);
            return _mapper.Map<IEnumerable<RequestClaimPart>>(entities);
        }

    }
}
