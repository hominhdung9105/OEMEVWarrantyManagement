using AutoMapper;
using OEMEVWarrantyManagement.Application.Dtos;
using OEMEVWarrantyManagement.Application.IRepository;
using OEMEVWarrantyManagement.Application.IServices;
using OEMEVWarrantyManagement.Domain.Entities;
using OEMEVWarrantyManagement.Share.Enum;
using System;
using System.Collections.Generic;
using System.IO;
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
        private readonly IWarrantyClaimRepository _warrantyClaimRepository;
        public ClaimPartService(IClaimPartRepository claimPartRepository, IMapper mapper, IPartRepository partRepository, IWarrantyClaimRepository warrantyClaimRepository)
        {
            _claimPartReposotory = claimPartRepository;
            _mapper = mapper;
            _partRepository = partRepository;
            _warrantyClaimRepository = warrantyClaimRepository;
        }

        public async Task<bool> CheckQuantityClaimPartAsync(Guid claimId)
        {
            var claimParts = await _claimPartReposotory.GetClaimPartByClaimIdAsync(claimId);
            var parts = await _partRepository.GetAllAsync();
            foreach (var cp in claimParts)
            {
                var part = parts.FirstOrDefault(p => p.PartId == cp.PartId);
                if (part == null)
                    return false;
                if (part.StockQuantity < cp.Quantity)
                    return false;
            }
            return true;
        }

        public async Task<RequestClaimPart> CreateClaimPartAsync(RequestClaimPart dto)
        {
            var entity = _mapper.Map<ClaimPart>(dto);
            entity.Status = "pending";
            var part = await _partRepository.GetPartsByIdAsync(entity.PartId);
            var create = await _claimPartReposotory.CreateClaimPartAsync(entity);
            return _mapper.Map<RequestClaimPart>(create);
        }

        public async Task<IEnumerable<RequestClaimPart>> GetClaimPartsAsync(Guid claimId)
        {
            var entities = await _claimPartReposotory.GetClaimPartByClaimIdAsync(claimId);
            return _mapper.Map<IEnumerable<RequestClaimPart>>(entities);
        }

        //TODO? - truyen them vao claimpartId
        //TODO - Chua xet neu claim aprove hay chua
        public async Task<bool> UpdateStatusClaimPartAsync(Guid claimId)
        {
            var claimParts = await _claimPartReposotory.GetClaimPartByClaimIdAsync(claimId);
            var parts = await _partRepository.GetAllAsync();
            var isEnoughStock = await CheckQuantityClaimPartAsync(claimId);
            string status = isEnoughStock ? "enough part" : "not enough part";
            var targetClaimParts = claimParts
                .Where(cp => cp.Status == "pending" && cp.Action == "repair")
                .ToList();

            if (isEnoughStock)
            {
                foreach (var cp in targetClaimParts)
                {
                    cp.Status = status;

                    var part = parts.FirstOrDefault(p => p.PartId == cp.PartId);
                    if (part != null)
                    {
                        part.StockQuantity -= cp.Quantity;
                        if (part.StockQuantity < 0)
                            part.StockQuantity = 0;
                    }
                }
                await _claimPartReposotory.UpdateRangeAsync(claimParts);
                await _partRepository.UpdateRangeAsync(parts);
            }
            return true;
        }

        //public async Task UpdateEnoughClaimPartsAsync()
        //{
        //    //list cac claimpart co status = "not enough part"
        //    var notEnoughPart = await _claimPartReposotory.GetAllNotEnoughAsync();
        //    var parts = await _partRepository.GetAllAsync();
        //    foreach (var cp in notEnoughPart)
        //    {
        //        var part = parts.FirstOrDefault(p => p.PartId == cp.PartId);
        //        if (part != null)
        //        {
        //            if (part.StockQuantity >= cp.Quantity)
        //            {

        //                part.StockQuantity -= cp.Quantity;
        //                cp.Status = "enough part";
        //            }
        //        }
        //    }
        //    await _partRepository.UpdateRangeAsync(parts);
        //    await _claimPartReposotory.UpdateRangeAsync(notEnoughPart);
        //}


    }
}
