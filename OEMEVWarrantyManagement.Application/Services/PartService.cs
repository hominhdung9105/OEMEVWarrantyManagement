using AutoMapper;
using OEMEVWarrantyManagement.Application.Dtos;
using OEMEVWarrantyManagement.Application.IRepository;
using OEMEVWarrantyManagement.Application.IServices;
using OEMEVWarrantyManagement.Share.Exceptions;
using OEMEVWarrantyManagement.Share.Models.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OEMEVWarrantyManagement.Application.Services
{
    public class PartService : IPartService
    {
        private readonly IPartRepository _partRepository;
        private readonly IMapper _mapper;
        public PartService(IPartRepository partRepository, IMapper mapper)
        {
            _partRepository = partRepository;
            _mapper = mapper;
        }
        public async Task<IEnumerable<PartDto>> GetPartByOrgIdAsync(Guid id)
        {
            var entities = await _partRepository.GetByOrgIdAsync(id);
            if (!entities.Any())
            {
                throw new ApiException(ResponseError.NotFoundPartHere);
            }
            return _mapper.Map<IEnumerable<PartDto>>(entities);
        }

        public async Task<IEnumerable<PartDto>> GetAllAsync()
        {
            var entities = await _partRepository.GetAllAsync();
            return _mapper.Map<IEnumerable<PartDto>>(entities);
        }
    }
}
