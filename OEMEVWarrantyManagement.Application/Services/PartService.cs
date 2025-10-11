using AutoMapper;
using OEMEVWarrantyManagement.Application.Dtos;
using OEMEVWarrantyManagement.Application.IRepository;
using OEMEVWarrantyManagement.Application.IServices;
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
        private readonly CurrentUserService _currentUserService;
        public PartService(IPartRepository partRepository, IMapper mapper, IPartOrderItemRepository orderItemRepository, IPartOrderRepository orderRepository, IClaimPartRepository claimPartRepository, IWarrantyClaimRepository warrantyClaimRepository, CurrentUserService currentUserService)
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

        public async Task<IEnumerable<PartDto>> GetPartsAsync(string model, string category)
        {
            var parts = await _partRepository.GetPartsAsync(model, category);
            return _mapper.Map<IEnumerable<PartDto>>(parts);
        }

        public async Task<bool> CheckQuantityClaimPartAsync(Guid claimId)
        {
            var claimParts = await _claimPartRepository.GetClaimPartByClaimIdAsync(claimId);
            var parts = await _partRepository.GetAllAsync();
            foreach (var cp in claimParts)
            {
                var part = parts.FirstOrDefault(p => p.PartId == cp.PartId);
                if (part == null)
                    return false;
                if (part.StockQuantity < cp.Quantity)
                    return false;
            }
            return true;
        }


        public async Task<IEnumerable<PartDto>> UpdateQuantityAsync(Guid orderId)
        {
            var partOrderItem = await _orderItemRepository.GetAllByOrderIdAsync(orderId);
            var partOrder = await _orderRepository.GetPartOrderByIdAsync(orderId);
            var parts = await _partRepository.GetPartsAsync();

            if (partOrder.Status == "shipped")
            {
                foreach(var partDto in partOrderItem)
                {
                    var part = parts.FirstOrDefault(p => p.PartId == partDto.PartId);
                    if (part != null)
                    {
                        part.StockQuantity += partDto.Quantity;
                    }
                }
                await _partRepository.UpdateRangeAsync(parts);
                await UpdateEnoughClaimPartsAsync();
            }

            return _mapper.Map<IEnumerable<PartDto>>(parts);
        }

        //public async Task UpdateEnoughClaimPartsAsync()
        //{
        //    //list cac claimpart co status = "not enough part"
        //    var notEnoughPart = await _claimPartRepository.GetAllNotEnoughAsync();
        //    var parts = await _partRepository.GetAllAsync();
        //    foreach (var cp in notEnoughPart)
        //    {
        //        var part = parts.FirstOrDefault(p => p.PartId == cp.PartId);
        //        if (part != null)
        //        {
        //            if (part.StockQuantity >= cp.Quantity)
        //            {

        //                part.StockQuantity -= cp.Quantity;
        //                cp.Status = "enough part";
        //            }
        //        }
        //    }
        //    await _partRepository.UpdateRangeAsync(parts);
        //    await _claimPartRepository.UpdateRangeAsync(notEnoughPart);
        //}

        public async Task UpdateEnoughClaimPartsAsync()
        {
            //list cac claimpart co status = "not enough part"
            var notEnoughPart = await _claimPartRepository.GetAllNotEnoughAsync();
            var parts = await _partRepository.GetAllAsync();
            var staffId = _currentUserService.GetUserId().ToString();
            var warrantyClaims = await _claimRepository.GetAllWarrantyClaimAsync(staffId);
            //foreach (var cp in notEnoughPart)
            //{
            //    var part = parts.FirstOrDefault(p => p.PartId == cp.PartId);
            //    if (part != null)
            //    {
            //        if (part.StockQuantity >= cp.Quantity)
            //        {

            //            part.StockQuantity -= cp.Quantity;
            //            cp.Status = "enough part";
            //        }
            //    }
            //}

            foreach(var claim in warrantyClaims)
            {
                var isEnough = await CheckQuantityClaimPartAsync(claim.ClaimId);
                var claimParts = notEnoughPart.Where(cp => cp.ClaimId == claim.ClaimId);
                if (isEnough)
                {
                    foreach (var cp in claimParts)
                    {
                        var part = parts.FirstOrDefault(p => p.PartId == cp.PartId);
                        if (part != null)
                        {
                            part.StockQuantity -= cp.Quantity;
                            cp.Status = "enough part";
                        }
                    }
                }
                else
                {
                    foreach (var cp in claimParts)
                    {
                        cp.Status = "not enough part";
                    }
                }
            }
            await _partRepository.UpdateRangeAsync(parts);
            await _claimPartRepository.UpdateRangeAsync(notEnoughPart);
        }

    }
}
