using AutoMapper;
using OEMEVWarrantyManagement.Application.Dtos;
using OEMEVWarrantyManagement.Application.IRepository;
using OEMEVWarrantyManagement.Application.IServices;
using OEMEVWarrantyManagement.Domain.Entities;

namespace OEMEVWarrantyManagement.Application.Services
{
    public class VehicleService : IVehicleService
    {
        private readonly IVehicleRepository _vehicleRepository;
        private readonly IVehicleWarrantyPolicyRepository _vehicleWarrantyPolicyRepository;
        private readonly IMapper _mapper;
        private readonly IWarrantyPolicyRepository _warrantyPolicyRepository;
        private readonly ICustomerRepository _customerRepository;
        public VehicleService(IVehicleRepository vehicleRepository, IMapper mapper, IVehicleWarrantyPolicyRepository vehicleWarrantyPolicyRepository, IWarrantyPolicyRepository warrantyPolicyRepository, ICustomerRepository customerRepository)
        {
            _vehicleRepository = vehicleRepository;
            _mapper = mapper;
            _vehicleWarrantyPolicyRepository = vehicleWarrantyPolicyRepository;
            _warrantyPolicyRepository = warrantyPolicyRepository;
            _customerRepository = customerRepository;
        }
     
        public async Task<IEnumerable<ResponseVehicleDto>> GetAllVehicleAsync()
        {
            var entities = await _vehicleRepository.GetAllVehicleAsync();
            var results = _mapper.Map<IEnumerable<ResponseVehicleDto>>(entities);//Vin model year customerid
            //var customer = await 

            var allPolicies = await _warrantyPolicyRepository.GetAllAsync();//list policy

            foreach (var vehicle in results)
            {
                var vwps = await _vehicleWarrantyPolicyRepository.GetAllVehicleWarrantyPolicyByVinAsync(vehicle.Vin);

                foreach (var vwp in vwps)
                {
                    var policyInfo = allPolicies.FirstOrDefault(p => p.PolicyId == vwp.PolicyId);
                    if (policyInfo != null)
                    {
                        vehicle.PolicyInformation.Add(new PolicyInformationDto
                        {
                            PolicyName = policyInfo.Name,
                            StartDate = vwp.StartDate,
                            EndDate = vwp.EndDate
                        });
                    }
                }
                var customer = await _customerRepository.GetCustomerByIdAsync(vehicle.CustomerId);
                vehicle.CustomerName = customer.Name;
                vehicle.CustomerPhoneNunmber = customer.Phone;
                vehicle.CustomerId = customer.CustomerId;
            }

            return results;
        }

    }

}

