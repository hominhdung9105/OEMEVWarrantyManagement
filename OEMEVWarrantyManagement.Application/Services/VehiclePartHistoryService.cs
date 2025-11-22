using AutoMapper;
using OEMEVWarrantyManagement.Application.IRepository;
using OEMEVWarrantyManagement.Application.IServices;
using OEMEVWarrantyManagement.Application.Dtos;
using OEMEVWarrantyManagement.Share.Exceptions;
using OEMEVWarrantyManagement.Share.Models.Response;

namespace OEMEVWarrantyManagement.Application.Services
{
    public class VehiclePartHistoryService : IVehiclePartHistoryService
    {
        private readonly IVehiclePartHistoryRepository _repository;
        private readonly IVehicleRepository _vehicleRepository;
        private readonly ICustomerRepository _customerRepository;
        private readonly IMapper _mapper;
        public VehiclePartHistoryService(IVehiclePartHistoryRepository repository, IMapper mapper, IVehicleRepository vehicleRepository, ICustomerRepository customerRepository)
        {
            _repository = repository;
            _mapper = mapper;
            _vehicleRepository = vehicleRepository;
            _customerRepository = customerRepository;
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
    }
}