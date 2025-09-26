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
            CreateMap<WarrantyRequest, WarrantyRequestDto>();
            CreateMap<WarrantyRequestDto, WarrantyRequest>(); 
            CreateMap<WarrantyRecord, WarrantyRecordDto>();
            CreateMap<WarrantyRecordDto, WarrantyRecord>();
            CreateMap<CarConditionCurrent, CarConditionCurrentDto>();
            CreateMap<CarConditionCurrentDto, CarConditionCurrent>();
        }
    }
}
