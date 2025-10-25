using System.Collections.Generic;
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
            var result = _mapper.Map<IEnumerable<PartDto>>(entities);
            if (!entities.Any())
            {
                throw new ApiException(ResponseError.NotFoundPartHere);
            }
            foreach (var part in result)
            {
                if (part.StockQuantity > 0 && part.StockQuantity < 10)
                {
                    part.Status = PartStatus.LowStock.GetPartStatus();
                }
                else if (part.StockQuantity == 0)
                {
                    part.Status = PartStatus.OutOfStock.GetPartStatus();
                }
                else
                {
                    part.Status = PartStatus.InStock.GetPartStatus();
                }
            }
            return result;
        }

        public async Task<IEnumerable<PartDto>> GetAllAsync()
        {
            var entities = await _partRepository.GetAllAsync();
            return _mapper.Map<IEnumerable<PartDto>>(entities);
        }
        //K dùng =  xoá
        public async Task<IEnumerable<PartDto>> GetPartsAsync(string model)
        {
            var orgId = await _currentUserService.GetOrgId();

            var parts = await _partRepository.GetPartsAsync(model, orgId);
            return _mapper.Map<IEnumerable<PartDto>>(parts);
        }

        public async Task<IEnumerable<PartDto>> UpdateQuantityAsync(Guid orderId)
        {
            var partOrderItems = await _orderItemRepository.GetAllByOrderIdAsync(orderId);
            var orgId = await _currentUserService.GetOrgId();
            var parts = await _partRepository.GetByOrgIdAsync(orgId);

            // Group received quantities by model
            var quantityByModel = partOrderItems
                .GroupBy(i => i.Model)
                .Select(g => new { Model = g.Key, Total = g.Sum(x => x.Quantity) })
                .ToList();

            foreach (var item in quantityByModel)
            {
                var part = parts.FirstOrDefault(p => p.Model == item.Model);
                if (part != null)
                {
                    part.StockQuantity += item.Total;
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

                var isEnough = HasEnoughPartsForClaim(notEnoughParts, parts);

                if (!isEnough)
                    continue;

                // Gom nhóm theo model để tính số lượng yêu cầu
                var requiredByModel = claimParts
                    .GroupBy(cp => cp.Model)
                    .Select(g => new
                    {
                        Model = g.Key,
                        RequiredCount = g.Count()
                    })
                    .ToList();

                foreach (var req in requiredByModel)
                {
                    var part = parts.FirstOrDefault(p => p.Model == req.Model);

                    part.StockQuantity -= req.RequiredCount;
                }

                foreach (var cp in notEnoughParts)
                {
                    cp.Status = ClaimPartStatus.Enough.GetClaimPartStatus();
                }
                
                await _claimPartRepository.UpdateRangeAsync(notEnoughParts);
            }

            await _partRepository.UpdateRangeAsync(parts);
        }

        /// <summary>
        /// Kiểm tra xem tất cả các phụ tùng trong 1 claim có đủ tồn kho hay không.
        /// Gom nhóm theo Model để so sánh số lượng yêu cầu và tồn kho.
        /// </summary>
        public static bool HasEnoughPartsForClaim(IEnumerable<ClaimPart> claimParts, IEnumerable<Part> partsInStock)
        {
            // Gom nhóm theo model để tính số lượng yêu cầu
            var requiredByModel = claimParts
                .GroupBy(cp => cp.Model)
                .Select(g => new
                {
                    Model = g.Key,
                    RequiredCount = g.Count()
                })
                .ToList();

            // Kiểm tra từng model
            foreach (var req in requiredByModel)
            {
                var stock = partsInStock.FirstOrDefault(p => p.Model == req.Model);
                if (stock == null || stock.StockQuantity < req.RequiredCount)
                {
                    // Chỉ cần 1 cái không đủ là fail
                    return false;
                }
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

        public string? GetCategoryByModel(string model)
        {
            if (string.IsNullOrWhiteSpace(model) || !PartModel.IsValidModel(model))
                throw new ApiException(ResponseError.InvalidPartModel);

            return PartModel.GetCategoryByModel(model);
        }
    }
}
