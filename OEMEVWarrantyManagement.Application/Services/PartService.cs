using AutoMapper;
using OEMEVWarrantyManagement.Application.Dtos;
using OEMEVWarrantyManagement.Application.IRepository;
using OEMEVWarrantyManagement.Application.IServices;
using OEMEVWarrantyManagement.Domain.Entities;
using OEMEVWarrantyManagement.Share.Enums;
using OEMEVWarrantyManagement.Share.Exceptions;
using OEMEVWarrantyManagement.Share.Models.Response;

namespace OEMEVWarrantyManagement.Application.Services
{
    public class PartService : IPartService
    {
        private readonly IPartRepository _partRepository;
        private readonly IMapper _mapper;
        private readonly IPartOrderItemRepository _orderItemRepository;
        private readonly IPartOrderRepository _orderRepository;
        private readonly IWarrantyClaimRepository _claimRepository;
        private readonly IClaimPartRepository _claimPartRepository;
        private readonly ICurrentUserService _currentUserService;
        public PartService(IPartRepository partRepository, IMapper mapper, IPartOrderItemRepository orderItemRepository, IPartOrderRepository orderRepository, IClaimPartRepository claimPartRepository, IWarrantyClaimRepository warrantyClaimRepository, ICurrentUserService currentUserService)
        {
            _partRepository = partRepository;
            _mapper = mapper;
            _orderItemRepository = orderItemRepository;
            _orderRepository = orderRepository;
            _claimPartRepository = claimPartRepository;
            _claimRepository = warrantyClaimRepository;
            _currentUserService = currentUserService;
        }
        public async Task<IEnumerable<PartDto>> GetPartByOrgIdAsync(Guid id)
        {
            var entities = await _partRepository.GetByOrgIdAsync(id);
            if (!entities.Any())
            {
                throw new ApiException(ResponseError.NotFoundPartHere);
            }
            return _mapper.Map<IEnumerable<PartDto>>(entities);
        }

        public async Task<IEnumerable<PartDto>> GetAllAsync()
        {
            var entities = await _partRepository.GetAllAsync();
            return _mapper.Map<IEnumerable<PartDto>>(entities);
        }

        public async Task<IEnumerable<PartDto>> GetPartsAsync(string model)
        {
            var orgId = await _currentUserService.GetOrgId();

            var parts = await _partRepository.GetPartsAsync(model, orgId);
            return _mapper.Map<IEnumerable<PartDto>>(parts);
        }

        public async Task<IEnumerable<PartDto>> UpdateQuantityAsync(Guid orderId)
        {
            var partOrderItem = await _orderItemRepository.GetAllByOrderIdAsync(orderId);
            var orgId = await _currentUserService.GetOrgId();
            var parts = await _partRepository.GetByOrgIdAsync(orgId);

            foreach (var partDto in partOrderItem)
            {
                var part = parts.FirstOrDefault(p => p.PartId == partDto.PartId);
                if (part != null)
                {
                    part.StockQuantity += partDto.Quantity;
                }
            }

            await _partRepository.UpdateRangeAsync(parts);
            await UpdateEnoughClaimPartsAsync(orgId, parts);
            
            return _mapper.Map<IEnumerable<PartDto>>(parts);
        }

        public async Task UpdateEnoughClaimPartsAsync(Guid orgId, IEnumerable<Part> parts)
        {
            var warrantyClaims = await _claimRepository.GetAllWarrantyClaimByOrgIdAsync(orgId);

            warrantyClaims = warrantyClaims.Where(wc => wc.Status == WarrantyClaimStatus.Approved.GetWarrantyClaimStatus() || wc.Status == WarrantyClaimStatus.CarBackHome.GetWarrantyClaimStatus()).OrderBy(wc => wc.CreatedDate);

            foreach(var claim in warrantyClaims)
            {
                var claimParts = await _claimPartRepository.GetClaimPartByClaimIdAsync(claim.ClaimId);

                var notEnoughParts = claimParts.Where(cp => cp.Status == ClaimPartStatus.NotEnough.GetClaimPartStatus());

                if(!notEnoughParts.Any())
                    continue;

                var isEnough = CheckQuantityClaimPart(parts, notEnoughParts);

                if (isEnough)
                {

                    foreach (var cp in claimParts)
                    {
                        var part = parts.FirstOrDefault(p => p.Model == cp.Model);

                        part.StockQuantity -= cp.Quantity;
                        cp.Status = ClaimPartStatus.Enough.GetClaimPartStatus();
                    }
                }

                await _claimPartRepository.UpdateRangeAsync(notEnoughParts);
            }

            await _partRepository.UpdateRangeAsync(parts);
        }

        public static bool CheckQuantityClaimPart(IEnumerable<Part> parts, IEnumerable<ClaimPart> claimParts)
        {
            foreach (var cp in claimParts)
            {
                var part = parts.FirstOrDefault(p => p.Model == cp.Model);
                if (part == null || part.StockQuantity < cp.Quantity)
                    return false;
            }

            return true;
        }

        public IEnumerable<string> GetPartCategories()
        {
            return PartCategoryExtensions.GetAllCategories();
        }

        public IEnumerable<string> GetPartModels(string category)
        {
            if(string.IsNullOrWhiteSpace(category) || !PartCategoryExtensions.IsValidCategory(category))
                throw new ApiException(ResponseError.InvalidPartCategory);

            return PartModel.GetModels(category);
        }
    }
}
