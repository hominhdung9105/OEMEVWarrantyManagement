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
            CreateMap<WarrantyClaim, RequestWarrantyClaim>().ReverseMap();
            CreateMap<WarrantyClaim, ResponseWarrantyClaim>().ReverseMap();

            CreateMap<ClaimAttachment, ImageDto>().ReverseMap();

            CreateMap<Employee, EmployeeDto>().ReverseMap();
            CreateMap<Employee, AllTech>().ReverseMap();

            CreateMap<WorkOrder, WorkOrderDto>().ReverseMap();
            CreateMap<WorkOrder, RequestCreateWorkOrderDto>().ReverseMap();

            CreateMap<WarrantyPolicy, WarrantyPolicyDto>().ReverseMap();

            CreateMap<Part,  PartDto>().ReverseMap();

            CreateMap<ClaimPart, ClaimPartDto>().ReverseMap();
            CreateMap<ClaimPart, RequestClaimPart>().ReverseMap();

            CreateMap<PartOrder, PartOrderDto>().ReverseMap();
            CreateMap<PartOrder, RequestPartOrderDto>().ReverseMap();

            CreateMap<PartOrderItem, PartOrderItemDto>().ReverseMap();
            CreateMap<PartOrderItem, RequsetPartOrderItemDto>().ReverseMap();

            CreateMap<Vehicle, VehicleDto>().ReverseMap();
            CreateMap<Vehicle, ResponseVehicleDto>().ReverseMap();


        }
    }
}
