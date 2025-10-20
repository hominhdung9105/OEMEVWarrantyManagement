using AutoMapper;
using OEMEVWarrantyManagement.Application.Dtos;
using OEMEVWarrantyManagement.Application.IRepository;
using OEMEVWarrantyManagement.Application.IServices;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OEMEVWarrantyManagement.Application.Services
{
    public class VehicleWarrantyPolicyService : IVehicleWarrantyPolicyService
    {
        private readonly IVehicleWarrantyPolicyRepository _repository;
        private readonly IMapper _mapper;
        private readonly IWarrantyPolicyRepository _warrantyPolicyRepository;

        public VehicleWarrantyPolicyService(IVehicleWarrantyPolicyRepository repository, IMapper mapper, IWarrantyPolicyRepository warrantyPolicyRepository)
        {
            _repository = repository;
            _mapper = mapper;
            _warrantyPolicyRepository = warrantyPolicyRepository;
        }

        public async Task<IEnumerable<VehicleWarrantyPolicyDto>> GetAllByVinAsync(string vin)
        {
            var entities = await _repository.GetAllVehicleWarrantyPolicyByVinAsync(vin);

            // Filter only policies currently in effect (StartDate <= now <= EndDate)
            var now = DateTime.Now;
            var activeEntities = entities.Where(e => e.StartDate <= now && e.EndDate >= now).ToList();

            var policies = (await _warrantyPolicyRepository.GetAllAsync()).ToDictionary(p => p.PolicyId, p => p.Name);

            var dtos = activeEntities.Select(e => {
                var dto = _mapper.Map<VehicleWarrantyPolicyDto>(e);
                if (policies.ContainsKey(e.PolicyId)) dto.PolicyName = policies[e.PolicyId];
                return dto;
            });

            return dtos;
        }
    }
}
