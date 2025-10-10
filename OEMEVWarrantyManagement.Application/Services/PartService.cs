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
        private readonly IClaimPartRepository _claimPartRepository;
        public PartService(IPartRepository partRepository, IMapper mapper, IPartOrderItemRepository orderItemRepository, IPartOrderRepository orderRepository, IClaimPartRepository claimPartRepository)
        {
            _partRepository = partRepository;
            _mapper = mapper;
            _orderItemRepository = orderItemRepository;
            _orderRepository = orderRepository;
            _claimPartRepository = claimPartRepository;
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

        public Task UpdateQuantityAsync(IEnumerable<PartDto> entities)
        {
            throw new NotImplementedException();
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

        public async Task UpdateEnoughClaimPartsAsync()
        {
            //list cac claimpart co status = "not enough part"
            var notEnoughPart = await _claimPartRepository.GetAllNotEnoughAsync();
            var parts = await _partRepository.GetAllAsync();
            foreach (var cp in notEnoughPart)
            {
                var part = parts.FirstOrDefault(p => p.PartId == cp.PartId);
                if (part != null)
                {
                    if (part.StockQuantity >= cp.Quantity)
                    {

                        part.StockQuantity -= cp.Quantity;
                        cp.Status = "enough part";
                    }
                }
            }
            await _partRepository.UpdateRangeAsync(parts);
            await _claimPartRepository.UpdateRangeAsync(notEnoughPart);
        }
    }
}
