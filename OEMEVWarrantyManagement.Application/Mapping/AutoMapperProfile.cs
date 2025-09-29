using AutoMapper;
using OEMEVWarrantyManagement.Application.Dtos;
using OEMEVWarrantyManagement.Domain.Entities;

namespace OEMEVWarrantyManagement.Application.Mapping
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            //Warranty Request
            CreateMap<WarrantyClaim, WarrantyClaimDto>().ReverseMap();
            CreateMap<ClaimAttachment, ImageDto>().ReverseMap();
        }
    }
}
