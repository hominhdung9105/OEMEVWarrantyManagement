using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using OEMEVWarrantyManagement.Application.Dtos;
using OEMEVWarrantyManagement.Application.IRepository;
using OEMEVWarrantyManagement.Application.IServices;
using OEMEVWarrantyManagement.Domain.Entities;
using OEMEVWarrantyManagement.Share.Exceptions;
using OEMEVWarrantyManagement.Share.Models.Response;

namespace OEMEVWarrantyManagement.Application.Services
{
    public class ClaimPartService : IClaimPartService
    {
        private readonly IClaimPartRepository _claimPartRepository;
        private readonly IMapper _mapper;
        private readonly IPartRepository _partRepository;
        public ClaimPartService(IClaimPartRepository claimPartRepository, IMapper mapper, IPartRepository partRepository)
        {
            _claimPartRepository = claimPartRepository;
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

            var create = await _claimPartRepository.CreateClaimPartAsync(entity);
            return _mapper.Map<RequestClaimPart>(create);
        }

        public async Task<RequestClaimPart> UpdateSerialClaimPartAsync(Guid id, string serial) // TODO - chua cap nhat serial vao vehicle
        {
            var entity = _claimPartRepository.GetClaimPartAsync(id) ?? throw new ApiException(ResponseError.NotFoundClaimPart);
            entity.SerialNumber = serial;
            await _claimPartRepository.SaveChangesAsync();
            return _mapper.Map<Task<RequestClaimPart>>(entity);
        }
    }
}
