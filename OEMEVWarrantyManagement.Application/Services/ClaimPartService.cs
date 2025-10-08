using AutoMapper;
using OEMEVWarrantyManagement.Application.Dtos;
using OEMEVWarrantyManagement.Application.IRepository;
using OEMEVWarrantyManagement.Application.IServices;
using OEMEVWarrantyManagement.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OEMEVWarrantyManagement.Application.Services
{
    public class ClaimPartService : IClaimPartService
    {
        private readonly IClaimPartRepository _claimPartReposotory;
        private readonly IMapper _mapper;
        private readonly IPartRepository _partRepository;
        public ClaimPartService(IClaimPartRepository claimPartRepository, IMapper mapper, IPartRepository partRepository)
        {
            _claimPartReposotory = claimPartRepository;
            _mapper = mapper;
            _partRepository = partRepository;
        }

        public async Task<RequestClaimPart> CreateClaimPartAsync(RequestClaimPart dto)
        {
            var entity = _mapper.Map<ClaimPart>(dto);
            var part = await _partRepository.GetPartsByIdAsync(entity.PartId);
            if (part.StockQuantity < entity.Quantity)
            {
                entity.Status = "not enough parts";//TODO -ENUM (Thiếu, không có,....)
            }
            else entity.Status = "Wait for"; 

            var create = await _claimPartReposotory.CreateClaimPartAsync(entity);
            return _mapper.Map<RequestClaimPart>(create);
        }
    }
}
