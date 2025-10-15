using AutoMapper;
using OEMEVWarrantyManagement.Application.Dtos;
using OEMEVWarrantyManagement.Application.IRepository;
using OEMEVWarrantyManagement.Application.IServices;
using OEMEVWarrantyManagement.Domain.Entities;
using OEMEVWarrantyManagement.Share.Enums;

namespace OEMEVWarrantyManagement.Application.Services
{
    public class BackWarrantyClaimService : IBackWarrantyClaimService
    {
        private readonly IBackWarrantyClaimRepository _backWarrantyClaimRepository;
        private readonly IMapper _mapper;
        private readonly ICurrentUserService _currentUserService;
        private readonly IWarrantyClaimRepository _warrantyClaimRepository;
        public BackWarrantyClaimService(IBackWarrantyClaimRepository backWarrantyClaimRepository, IMapper mapper, ICurrentUserService currentUserService, IWarrantyClaimRepository warrantyClaimRepository)
        {
            _backWarrantyClaimRepository = backWarrantyClaimRepository;
            _mapper = mapper;
            _currentUserService = currentUserService;
            _warrantyClaimRepository = warrantyClaimRepository;
        }
        public async Task<BackWarrantyClaimDto> CreateBackWarrantyClaimAsync(CreateBackWarrantyClaimRequestDto request)
        {
            var entity = _mapper.Map<BackWarrantyClaim>(request);
            entity.CreatedDate = DateTime.Now;
            entity.CreatedByEmployeeId = _currentUserService.GetUserId();

            var warrantyClaim = await _warrantyClaimRepository.GetWarrantyClaimByIdAsync(entity.WarrantyClaimId);
            warrantyClaim.Status = WarrantyClaimStatus.WaitingForUnassigned.GetWarrantyClaimStatus();
            warrantyClaim.ConfirmBy = null;
            warrantyClaim.ConfirmDate = null;
            await _warrantyClaimRepository.UpdateAsync(warrantyClaim);

            var createdEntity = await _backWarrantyClaimRepository.CreateBackWarrantyClaimAsync(entity);
            return _mapper.Map<BackWarrantyClaimDto>(createdEntity);
        }

        public async Task<IEnumerable<BackWarrantyClaimDto>> GetAllBackWarrantyClaimsAsync()
        {
            var entities = await _backWarrantyClaimRepository.GetAllBackWarrantyClaimsAsync();
            //return entities.ContinueWith(t => _mapper.Map<IEnumerable<BackWarrantyClaimDto>>(t.Result));
            return _mapper.Map<IEnumerable<BackWarrantyClaimDto>>(entities);
        }
    }
}
