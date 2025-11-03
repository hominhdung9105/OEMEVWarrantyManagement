using AutoMapper;
using OEMEVWarrantyManagement.Application.Dtos;
using OEMEVWarrantyManagement.Application.IRepository;
using OEMEVWarrantyManagement.Application.IServices;
using OEMEVWarrantyManagement.Domain.Entities;
using OEMEVWarrantyManagement.Share.Enums;
using OEMEVWarrantyManagement.Share.Exceptions;
using OEMEVWarrantyManagement.Share.Models.Pagination;
using OEMEVWarrantyManagement.Share.Models.Response;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace OEMEVWarrantyManagement.Application.Services
{
    public class VehicleService : IVehicleService
    {
        private readonly IVehicleRepository _vehicleRepository;
        private readonly IVehicleWarrantyPolicyRepository _vehicleWarrantyPolicyRepository;
        private readonly IMapper _mapper;
        private readonly IWarrantyPolicyRepository _warrantyPolicyRepository;
        private readonly ICustomerRepository _customerRepository;
        private readonly ICurrentUserService _currentUserService;
        public VehicleService(IVehicleRepository vehicleRepository, IMapper mapper, IVehicleWarrantyPolicyRepository vehicleWarrantyPolicyRepository, IWarrantyPolicyRepository warrantyPolicyRepository, ICustomerRepository customerRepository, ICurrentUserService currentUserService)
        {
            _vehicleRepository = vehicleRepository;
            _mapper = mapper;
            _vehicleWarrantyPolicyRepository = vehicleWarrantyPolicyRepository;
            _warrantyPolicyRepository = warrantyPolicyRepository;
            _customerRepository = customerRepository;
            _currentUserService = currentUserService;
        }
     
        public async Task<PagedResult<ResponseVehicleDto>> GetPagedAsync(PaginationRequest request, string? search)
        {
            // Role-based access: sc tech forbidden; others allowed
            var role = _currentUserService.GetRole();
            if (role == RoleIdEnum.Technician.GetRoleId())
            {
                throw new ApiException(OEMEVWarrantyManagement.Share.Models.Response.ResponseError.Forbidden);
            }

            var (entities, totalRecords) = await _vehicleRepository.GetPagedVehicleAsync(request.Page, request.Size, search);
            var totalPages = (int)Math.Ceiling(totalRecords / (double) request.Size);

            var results = _mapper.Map<IEnumerable<ResponseVehicleDto>>(entities);

            var allPolicies = await _warrantyPolicyRepository.GetAllAsync();

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

            return new PagedResult<ResponseVehicleDto>
            {
                PageNumber = request.Page,
                PageSize = request.Size,
                TotalRecords = totalRecords,
                TotalPages = totalPages,
                Items = results
            };
        }

    }

}

