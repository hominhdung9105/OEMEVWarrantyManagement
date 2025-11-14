using AutoMapper;
using OEMEVWarrantyManagement.Application.Dtos;
using OEMEVWarrantyManagement.Application.IRepository;
using OEMEVWarrantyManagement.Application.IServices;
using OEMEVWarrantyManagement.Share.Models.Pagination;
using OEMEVWarrantyManagement.Share.Exceptions;
using OEMEVWarrantyManagement.Share.Models.Response;
using Microsoft.EntityFrameworkCore;
using OEMEVWarrantyManagement.Domain.Entities;


namespace OEMEVWarrantyManagement.Application.Services
{
    public class WarrantyPolicyService : IWarrantyPolicyService
    {
        private readonly IWarrantyPolicyRepository _warrantyPolicyRepository;
        private readonly IMapper _mapper;
        private readonly ICurrentUserService _currentUserService;
        public WarrantyPolicyService(IWarrantyPolicyRepository warrantyPolicyRepository, IMapper mapper, ICurrentUserService currentUserService)
        {
            _warrantyPolicyRepository = warrantyPolicyRepository;
            _mapper = mapper;
            _currentUserService = currentUserService;
        }
        public async Task<IEnumerable<WarrantyPolicyDto>> GetAllAsync()
        {
            var entities = await _warrantyPolicyRepository.GetAllAsync();
            return _mapper.Map<IEnumerable<WarrantyPolicyDto>>(entities);
        }

        public async Task<PagedResult<WarrantyPolicyDto>> GetAllAsync(PaginationRequest request)
        {
            var query = _warrantyPolicyRepository.Query();

            var total = await query.LongCountAsync();

            var items = await query
                .Skip(request.Page * request.Size)
                .Take(request.Size)
                .ToListAsync();

            var dtoItems = _mapper.Map<List<WarrantyPolicyDto>>(items);

            return new PagedResult<WarrantyPolicyDto>
            {
                PageNumber = request.Page,
                PageSize = request.Size,
                TotalRecords = total,
                TotalPages = (int)Math.Ceiling(total / (double)request.Size),
                Items = dtoItems
            };
        }

        public async Task<WarrantyPolicyDto?> GetByIdAsync(Guid id)
        {
            var entity = await _warrantyPolicyRepository.GetByIdAsync(id);
            return _mapper.Map<WarrantyPolicyDto>(entity);
        }

        public async Task<WarrantyPolicyCreateDto> CreateAsync(WarrantyPolicyCreateDto request)
        {
            var orgId = await _currentUserService.GetOrgId();
            var entity = _mapper.Map<WarrantyPolicy>(request);
            entity.OrganizationOrgId =(Guid)orgId;
            entity.Status = "Active";
            var created = await _warrantyPolicyRepository.AddAsync(entity);
            return _mapper.Map<WarrantyPolicyCreateDto>(created);
        }

        public async Task<WarrantyPolicyUpdateDto> UpdateAsync(Guid id, WarrantyPolicyUpdateDto request)
        {
            var entity = await _warrantyPolicyRepository.GetByIdAsync(id) ?? throw new ApiException(ResponseError.NotFoundWarrantyPolicy);
            entity.Name = request.Name;
            entity.CoveragePeriodMonths = request.CoveragePeriodMonths;
            entity.Conditions = request.Conditions;
            entity.Status = request.Status;
            if(entity.Status != "Active" && entity.Status != "Inactive")
            {
                throw new ApiException(ResponseError.InvalidPolicy);
            }
            //var update = _mapper.Map<WarrantyPolicy>(request);
            var updated = await _warrantyPolicyRepository.UpdateAsync(entity);
            return _mapper.Map<WarrantyPolicyUpdateDto>(updated);
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            return await _warrantyPolicyRepository.DeleteAsync(id);
        }
    }
}
