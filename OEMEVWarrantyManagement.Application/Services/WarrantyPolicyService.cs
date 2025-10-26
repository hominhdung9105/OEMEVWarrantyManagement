//using AutoMapper;
//using OEMEVWarrantyManagement.Application.Dtos;
//using OEMEVWarrantyManagement.Application.IRepository;
//using OEMEVWarrantyManagement.Application.IServices;
//using OEMEVWarrantyManagement.Share.Models.Pagination;
//using OEMEVWarrantyManagement.Share.Exceptions;
//using OEMEVWarrantyManagement.Share.Models.Response;
//using Microsoft.EntityFrameworkCore;


//namespace OEMEVWarrantyManagement.Application.Services
//{
//    public class WarrantyPolicyService : IWarrantyPolicyService
//    {
//        private readonly IWarrantyPolicyRepository _warrantyPolicyRepository;
//        private readonly IMapper _mapper;
//        public WarrantyPolicyService(IWarrantyPolicyRepository warrantyPolicyRepository, IMapper mapper)
//        {
//            _warrantyPolicyRepository = warrantyPolicyRepository;
//            _mapper = mapper;
//        }
//        public async Task<IEnumerable<WarrantyPolicyDto>> GetAllAsync()
//        {
//            var entities = await _warrantyPolicyRepository.GetAllAsync();
//            return _mapper.Map<IEnumerable<WarrantyPolicyDto>>(entities);
//        }

//        public async Task<PagedResult<WarrantyPolicyDto>> GetAllAsync(PaginationRequest request)
//        {
//            var query = _warrantyPolicyRepository.Query();

//            var total = await query.LongCountAsync();

//            var items = await query
//                .OrderByCreationAscending()
//                .Skip(request.Page * request.Size)
//                .Take(request.Size)
//                .ToListAsync();

//            var dtoItems = _mapper.Map<List<WarrantyPolicyDto>>(items);

//            return new PagedResult<WarrantyPolicyDto>
//            {
//                Page = request.Page,
//                Size = request.Size,
//                TotalItems = total,
//                TotalPages = (int)Math.Ceiling(total / (double)request.Size),
//                Items = dtoItems
//            };
//        }
//    }
//}
