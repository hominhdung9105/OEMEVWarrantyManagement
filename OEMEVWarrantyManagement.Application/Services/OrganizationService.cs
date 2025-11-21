using AutoMapper;
using OEMEVWarrantyManagement.Application.Dtos;
using OEMEVWarrantyManagement.Application.IRepository;
using OEMEVWarrantyManagement.Application.IServices;

namespace OEMEVWarrantyManagement.Application.Services
{
    public class OrganizationService : IOrganizationService
    {
        private readonly IOrganizationRepository _organizationRepository;
        private readonly IMapper _mapper;
        public OrganizationService(IOrganizationRepository organizationRepository, IMapper mapper)
        {
            _organizationRepository = organizationRepository;
            _mapper = mapper;
        }
        public async Task<IEnumerable<OrganizationDto>> GetAllOrganizationByAsync()
        {
            var entities = await _organizationRepository.GetAllOrganizationsAsync();
            return _mapper.Map<IEnumerable<OrganizationDto>>(entities);
        }
    }
}
