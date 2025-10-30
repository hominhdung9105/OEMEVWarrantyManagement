using AutoMapper;
using OEMEVWarrantyManagement.Application.Dtos;
using OEMEVWarrantyManagement.Domain.Entities;

namespace OEMEVWarrantyManagement.Application.Mapping
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<WarrantyClaim, WarrantyClaimDto>().ReverseMap();
            CreateMap<WarrantyClaim, RequestWarrantyClaim>().ReverseMap();
            CreateMap<WarrantyClaim, ResponseWarrantyClaim>().ReverseMap();
            CreateMap<WarrantyClaim, ResponseWarrantyClaimDto>().ReverseMap();

            CreateMap<WarrantyClaim, WarrantyPolicyDto>().ReverseMap();

            CreateMap<ClaimAttachment, ImageDto>().ReverseMap();

            CreateMap<Employee, EmployeeDto>().ReverseMap();
            CreateMap<Employee, AllTech>().ReverseMap();

            CreateMap<WorkOrder, WorkOrderDto>().ReverseMap();
            CreateMap<WorkOrder, WorkOrderDetailDto>().ReverseMap();
            CreateMap<WorkOrder, RequestCreateWorkOrderDto>().ReverseMap();

            CreateMap<VehicleWarrantyPolicy, VehicleWarrantyPolicyDto>().ReverseMap();

            CreateMap<WarrantyPolicy, WarrantyPolicyDto>().ReverseMap();
            CreateMap<WarrantyPolicy, VehicleWarrantyPolicy>().ReverseMap();

            CreateMap<Part, PartDto>().ReverseMap();

            CreateMap<ClaimPart, ClaimPartDto>().ReverseMap();
            CreateMap<ClaimPart, RequestClaimPart>().ReverseMap();
            CreateMap<ClaimPart, ShowClaimPartDto>().ReverseMap();

            CreateMap<PartOrder, PartOrderDto>().ReverseMap();
            CreateMap<PartOrder, RequestPartOrderDto>().ReverseMap();
            CreateMap<PartOrder, ResponsePartOrderDto>().ReverseMap();
            CreateMap<PartOrder, ResponsePartOrderForScStaffDto>().ReverseMap();

            CreateMap<PartOrderItem, PartOrderItemDto>().ReverseMap();
            CreateMap<PartOrderItem, RequsetPartOrderItemDto>().ReverseMap();
            CreateMap<PartOrderItem, ResponsePartOrderItemDto>().ReverseMap();
            CreateMap<PartOrderItem, ResponsePartOrderItemForScStaffDto>().ReverseMap();

            CreateMap<Vehicle, VehicleDto>().ReverseMap();
            CreateMap<Vehicle, ResponseVehicleDto>().ReverseMap();

            CreateMap<BackWarrantyClaim, BackWarrantyClaimDto>().ReverseMap();
            CreateMap<BackWarrantyClaim, CreateBackWarrantyClaimRequestDto>().ReverseMap();

            CreateMap<Campaign, CampaignDto>().ReverseMap();
            CreateMap<Campaign, RequestCampaignDto>().ReverseMap();

            CreateMap<Customer, CustomerDto>().ReverseMap();

            CreateMap<CampaignVehicle, CampaignVehicleDto>()
                .ForMember(d => d.Vehicle, opt => opt.MapFrom(src => src.Vehicle))
                .ForMember(d => d.Customer, opt => opt.MapFrom(src => src.Vehicle != null ? src.Vehicle.Customer : null))
                .ReverseMap();
        }
    }
}
