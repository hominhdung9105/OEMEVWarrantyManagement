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
            if (request.Status != "Active" && request.Status != "Inactive")
            {
                throw new ApiException(ResponseError.InvalidPolicy);
            }
            var entity = await _warrantyPolicyRepository.GetByIdAsync(id) ?? throw new ApiException(ResponseError.NotFoundWarrantyPolicy);

            var orgId = await _currentUserService.GetOrgId();
            request.OrgId = orgId;
            request.PolicyId = id;

            _mapper.Map(request, entity);

            var updated = await _warrantyPolicyRepository.UpdateAsync(entity);
            return _mapper.Map<WarrantyPolicyUpdateDto>(updated);
        }

        public async Task<bool> SetPolicyStatusAsync(Guid id, bool isActive)
        {
            return await _warrantyPolicyRepository.SetPolicyStatusAsync(id, isActive);
        }
    }
}
