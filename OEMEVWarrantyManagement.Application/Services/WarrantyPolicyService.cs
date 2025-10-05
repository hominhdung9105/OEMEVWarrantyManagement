using AutoMapper;
using OEMEVWarrantyManagement.Application.Dtos;
using OEMEVWarrantyManagement.Application.IRepository;
using OEMEVWarrantyManagement.Application.IServices;
using OEMEVWarrantyManagement.Share.Exceptions;
using OEMEVWarrantyManagement.Share.Models.Response;


namespace OEMEVWarrantyManagement.Application.Services
{
    public class WarrantyPolicyService : IWarrantyPolicyService
    {
        private readonly IWarrantyPolicyRepository _warrantyPolicyRepository;
        private readonly IMapper _mapper;
        public WarrantyPolicyService(IWarrantyPolicyRepository warrantyPolicyRepository, IMapper mapper)
        {
            _warrantyPolicyRepository = warrantyPolicyRepository;
            _mapper = mapper;
        }
        public async Task<IEnumerable<WarrantyPolicyDto>> GetAllAsync()
        {
            var entities = await _warrantyPolicyRepository.GetAllAsync();
            return _mapper.Map<IEnumerable<WarrantyPolicyDto>>(entities);
        }
    }
}
