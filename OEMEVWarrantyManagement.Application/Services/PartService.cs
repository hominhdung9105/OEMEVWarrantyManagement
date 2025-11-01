using System.Collections.Generic;
using AutoMapper;
using OEMEVWarrantyManagement.Application.Dtos;
using OEMEVWarrantyManagement.Application.IRepository;
using OEMEVWarrantyManagement.Application.IServices;
using OEMEVWarrantyManagement.Domain.Entities;
using OEMEVWarrantyManagement.Share.Enums;
using OEMEVWarrantyManagement.Share.Exceptions;
using OEMEVWarrantyManagement.Share.Models.Pagination;
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

        public async Task<PagedResult<PartDto>> GetPagedAsync(PaginationRequest request, string? search = null, string? status = null)
        {
            var orgId = await _currentUserService.GetOrgId();

            // If no filters, use repository-level paging as-is
            if (string.IsNullOrWhiteSpace(search) && string.IsNullOrWhiteSpace(status))
            {
                var (entities, totalRecords) = await _partRepository.GetPagedPartAsync(request.Page, request.Size, orgId);
                var pageItems = _mapper.Map<IEnumerable<PartDto>>(entities).ToList();

                // compute status for paged items
                foreach (var part in pageItems)
                {
                    if (part.StockQuantity > 0 && part.StockQuantity < 10)
                        part.Status = PartStatus.LowStock.GetPartStatus();
                    else if (part.StockQuantity == 0)
                        part.Status = PartStatus.OutOfStock.GetPartStatus();
                    else
                        part.Status = PartStatus.InStock.GetPartStatus();
                }

                var totalPages = (int)Math.Ceiling(totalRecords / (double)request.Size);
                return new PagedResult<PartDto>
                {
                    PageNumber = request.Page,
                    PageSize = request.Size,
                    TotalRecords = totalRecords,
                    TotalPages = totalPages,
                    Items = pageItems
                };
            }

            // With filters: build query first, then apply pagination once
            var query = _partRepository.QueryByOrgId(orgId);

            if (!string.IsNullOrWhiteSpace(search))
            {
                var s = search.Trim();
                query = query.Where(p => p.Model.Contains(s) || p.Category.Contains(s));
            }

            if (!string.IsNullOrWhiteSpace(status))
            {
                var st = status.Trim();
                if (string.Equals(st, PartStatus.OutOfStock.GetPartStatus(), StringComparison.OrdinalIgnoreCase))
                {
                    query = query.Where(p => p.StockQuantity == 0);
                }
                else if (string.Equals(st, PartStatus.LowStock.GetPartStatus(), StringComparison.OrdinalIgnoreCase))
                {
                    query = query.Where(p => p.StockQuantity > 0 && p.StockQuantity < 10);
                }
                else if (string.Equals(st, PartStatus.InStock.GetPartStatus(), StringComparison.OrdinalIgnoreCase))
                {
                    query = query.Where(p => p.StockQuantity >= 10);
                }
            }

            var filteredTotal = query.Count();
            var entitiesFiltered = query
                .OrderBy(p => p.Model)
                .Skip(request.Page * request.Size)
                .Take(request.Size)
                .ToList();

            var items = _mapper.Map<IEnumerable<PartDto>>(entitiesFiltered).ToList();
            foreach (var part in items)
            {
                if (part.StockQuantity > 0 && part.StockQuantity < 10)
                    part.Status = PartStatus.LowStock.GetPartStatus();
                else if (part.StockQuantity == 0)
                    part.Status = PartStatus.OutOfStock.GetPartStatus();
                else
                    part.Status = PartStatus.InStock.GetPartStatus();
            }

            return new PagedResult<PartDto>
            {
                PageNumber = request.Page,
                PageSize = request.Size,
                TotalRecords = filteredTotal,
                TotalPages = (int)Math.Ceiling(filteredTotal / (double)request.Size),
                Items = items
            };
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

        public async Task<IEnumerable<PartDto>> UpdateEvmQuantityAsync(Guid orderId)
        {
            var orgId = await _currentUserService.GetOrgId();
            var partOrderItems = await _orderItemRepository.GetAllByOrderIdAsync(orderId);
            var quantityByModel = partOrderItems
               .GroupBy(i => i.Model)
               .Select(g => new { Model = g.Key, Total = g.Sum(x => x.Quantity) })
               .ToList();
            var parts = await _partRepository.GetByOrgIdAsync(orgId);
            var updatedParts = new List<Part>();
            foreach (var item in quantityByModel)
            {
                var part = parts.FirstOrDefault(p => p.Model == item.Model);
                if (part == null)
                    throw new ApiException(ResponseError.InvalidPartModel);

                if (part.StockQuantity < item.Total)
                    throw new ApiException(ResponseError.NotEnoughStock);

                part.StockQuantity -= item.Total;
                updatedParts.Add(part);
            }
            await _partRepository.UpdateRangeAsync(updatedParts);
            return _mapper.Map<IEnumerable<PartDto>>(updatedParts);
        }
    }
}
