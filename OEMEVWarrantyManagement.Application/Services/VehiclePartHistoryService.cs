using AutoMapper;
using OEMEVWarrantyManagement.Application.Dtos;
using OEMEVWarrantyManagement.Application.IRepository;
using OEMEVWarrantyManagement.Application.IServices;
using OEMEVWarrantyManagement.Share.Enums;
using OEMEVWarrantyManagement.Share.Exceptions;
using OEMEVWarrantyManagement.Share.Models.Pagination;
using OEMEVWarrantyManagement.Share.Models.Response;

namespace OEMEVWarrantyManagement.Application.Services
{
    public class VehiclePartHistoryService : IVehiclePartHistoryService
    {
        private readonly IVehiclePartHistoryRepository _repository;
        private readonly IVehicleRepository _vehicleRepository;
        private readonly ICustomerRepository _customerRepository;
        private readonly IMapper _mapper;
        private readonly ICurrentUserService _currentUserService;

        public VehiclePartHistoryService(IVehiclePartHistoryRepository repository, IMapper mapper, IVehicleRepository vehicleRepository, ICustomerRepository customerRepository, ICurrentUserService currentUserService)
        {
            _repository = repository;
            _mapper = mapper;
            _vehicleRepository = vehicleRepository;
            _customerRepository = customerRepository;
            _currentUserService = currentUserService;
        }

        public async Task<IEnumerable<VehiclePartHistoryDto>> GetHistoryByVinAsync(string vin)
        {
            var entities = await _repository.GetByVinAsync(vin);
            return _mapper.Map<IEnumerable<VehiclePartHistoryDto>>(entities);
        }

        public async Task<IEnumerable<VehiclePartHistoryDto>> GetHistoryByVinAndModelAsync(string vin, string model)
        {
            var entities = await _repository.GetByVinAndModelAsync(vin, model);
            return _mapper.Map<IEnumerable<VehiclePartHistoryDto>>(entities);
        }

        public async Task<VehicleWithHistoryDto> GetVehicleWithHistoryAsync(string vin, string? model = null)
        {
            if (string.IsNullOrWhiteSpace(vin)) throw new ApiException(ResponseError.NotfoundVin);
            var vehicle = await _vehicleRepository.GetVehicleByVinAsync(vin) ?? throw new ApiException(ResponseError.NotfoundVin);

            IEnumerable<VehiclePartHistoryDto> histories = string.IsNullOrWhiteSpace(model)
                ? await GetHistoryByVinAsync(vin)
                : await GetHistoryByVinAndModelAsync(vin, model!);

            var dto = _mapper.Map<VehicleWithHistoryDto>(vehicle);
            dto.Histories = histories.ToList();

            var customer = await _customerRepository.GetCustomerByIdAsync(vehicle.CustomerId);
            if (customer != null)
            {
                dto.CustomerName = customer.Name;
                dto.CustomerPhone = customer.Phone;
            }
            return dto;
        }

        public async Task<IEnumerable<string>> GetSerialsByVinAndPartModelAsync(string vin, string partModel)
        {
            var vehicle = await _vehicleRepository.GetVehicleByVinAsync(vin) ?? throw new ApiException(ResponseError.NotfoundVin);

            // Validate part model using PartModel.IsValidModel
            if (!PartModel.IsValidModel(partModel))
                throw new ApiException(ResponseError.InvalidPartModel);

            var parts = await _repository.GetByVinAndModelAsync(vin, partModel);
            return parts.Select(p => p.SerialNumber);
        }

        // Updated: accept unified search parameter and use DB-side unified paging similar to WarrantyClaimService
        public async Task<PagedResult<ResponseVehiclePartHistoryDto>> GetPagedAsync(PaginationRequest request, string? search = null, string? condition = null, string? status = null)
        {
            // Role-based access: evm + admin => all; sc staff => org only; tech => forbidden
            var role = _currentUserService.GetRole();
            Guid? orgId = null;

            if (role == RoleIdEnum.Technician.GetRoleId())
            {
                throw new ApiException(ResponseError.Forbidden);
            }
            else if (role == RoleIdEnum.ScStaff.GetRoleId())
            {
                orgId = await _currentUserService.GetOrgId();
            }
            // Admin or EvmStaff: orgId remains null (no restriction)

            var (entities, totalRecords) = await _repository.GetPagedUnifiedAsync(request, orgId, search, condition, status);

            var totalPages = (int)Math.Ceiling(totalRecords / (double)request.Size);

            var items = new List<ResponseVehiclePartHistoryDto>();

            // Batch load vehicles and customers to avoid N+1
            var vins = entities.Select(e => e.Vin).Where(v => !string.IsNullOrWhiteSpace(v)).Distinct().ToList();
            var vehicles = await _vehicleRepository.GetVehiclesByVinsAsync(vins);
            var vehicleDict = vehicles.ToDictionary(v => v.Vin);

            var customerIds = vehicles.Select(v => v.CustomerId).Distinct().ToList();
            var customers = await _customerRepository.GetCustomersByIdsAsync(customerIds);
            var customerDict = customers.ToDictionary(c => c.CustomerId);

            foreach (var item in entities)
            {
                var dto = _mapper.Map<ResponseVehiclePartHistoryDto>(item);

                if (!string.IsNullOrWhiteSpace(item.Vin) && vehicleDict.TryGetValue(item.Vin, out var veh))
                {
                    dto.CarModel = veh.Model;
                    dto.CarYear = veh.Year.ToString();

                    if (customerDict.TryGetValue(veh.CustomerId, out var cust))
                    {
                        dto.CustomerName = cust.Name;
                        dto.CustomerPhone = cust.Phone;
                        dto.CustomerEmail = cust.Email;
                    }
                }

                items.Add(dto);
            }

            return new PagedResult<ResponseVehiclePartHistoryDto>
            {
                PageNumber = request.Page,
                PageSize = request.Size,
                TotalRecords = (int)totalRecords,
                TotalPages = totalPages,
                Items = items
            };
        }
    }
}