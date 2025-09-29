using AutoMapper;
using OEMEVWarrantyManagement.Application.Dtos;
using OEMEVWarrantyManagement.Application.IRepository;
using OEMEVWarrantyManagement.Application.IServices;
using OEMEVWarrantyManagement.Domain.Entities;
using OEMEVWarrantyManagement.Share.Exceptions;
using OEMEVWarrantyManagement.Share.Models.Response;

namespace OEMEVWarrantyManagement.Application.Services
{
    public class WarrantyClaimService : IWarrantyClaimService
    {
        private readonly IMapper _mapper;
        private readonly IWarrantyClaimRepository _warrantyClaimRepository;
        private readonly IVehicleRepository _vehicleRepository;
        public WarrantyClaimService(IMapper mapper, IWarrantyClaimRepository warrantyClaimRepository, IVehicleRepository vehicleRepository)
        {
            _mapper = mapper;
            _warrantyClaimRepository = warrantyClaimRepository;
            _vehicleRepository = vehicleRepository;
        }
        
        public async Task<WarrantyClaimDto> CreateAsync(WarrantyClaimDto request)
        {
            var exist = await _vehicleRepository.GetVehicleByVinAsync(request.Vin) ?? throw new ApiException(ResponseError.NotfoundVin);
            var entity = _mapper.Map<WarrantyClaim>(request);
            var result = await _warrantyClaimRepository.CreateAsync(entity);
            return _mapper.Map<WarrantyClaimDto>(result);
        }

        public async Task<bool> DeleteAsync(Guid claimId)
        {
            var entity = await _warrantyClaimRepository
                .GetWarrantyClaimByIdAsync(claimId) ?? throw new ApiException(ResponseError.NotFoundWarrantyClaim);
            var result = await _warrantyClaimRepository.DeleteAsync(entity);
            return true;
        }

        public async Task<IEnumerable<WarrantyClaimDto>> GetAllWarrantyClaimAsync()
        {
            var entities = await _warrantyClaimRepository.GetAllWarrantyClaimAsync();
            return _mapper.Map<IEnumerable<WarrantyClaimDto>>(entities);
        }

        public async Task<IEnumerable<WarrantyClaimDto>> GetAllWarrantyClaimAsync(string staffId)
        {
            var entities = await _warrantyClaimRepository.GetAllWarrantyClaimAsync(staffId);
            return _mapper.Map<IEnumerable<WarrantyClaimDto>>(entities);
        }

        public async Task<IEnumerable<WarrantyClaimDto>> GetWarrantyClaimByVinAsync(string vin, string staffId)
        {
            var entities = await _warrantyClaimRepository.GetWarrantyClaimByVinAsync(vin, staffId);
            if (!entities.Any())
            {
                throw new ApiException(ResponseError.NotfoundVin);
            }
            return _mapper.Map<IEnumerable<WarrantyClaimDto>>(entities);
        }

        public async Task<IEnumerable<WarrantyClaimDto>> GetWarrantyClaimByVinAsync(string vin)
        {
            var entities = await _warrantyClaimRepository.GetWarrantyClaimByVinAsync(vin);
            if (!entities.Any())
            {
                throw new ApiException(ResponseError.NotfoundVin);
            }
            return _mapper.Map<IEnumerable<WarrantyClaimDto>>(entities);
        }

        public async Task<WarrantyClaimDto> UpdateAsync(WarrantyClaimDto dto)
        {
            var exist = await _warrantyClaimRepository
                .GetWarrantyClaimByIdAsync((Guid)dto.ClaimId) ?? throw new ApiException(ResponseError.NotFoundWarrantyClaim);
            //if (dto.Vin != null) exist.Vin = dto.Vin;
            //if (dto.ServiceCenterId != null) exist.ServiceCenterId = dto.ServiceCenterId;
            if (dto.Status != null) exist.Status = dto.Status;


            if (dto.Description != null) exist.Description = dto.Description;


            if (await _vehicleRepository.GetVehicleByVinAsync(exist.Vin) == null) throw new ApiException(ResponseError.NotfoundVin);
            var update = await _warrantyClaimRepository.UpdateAsync(exist);
            return _mapper.Map<WarrantyClaimDto>(update);
        }
    }
}
