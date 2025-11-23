using AutoMapper;
using OEMEVWarrantyManagement.Application.Dtos;
using OEMEVWarrantyManagement.Domain.Entities;
using System.Text.Json;

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
            CreateMap<Employee, CreateEmployeeDto>().ReverseMap();
            CreateMap<Employee, UpdateEmployeeDto>().ReverseMap();

            CreateMap<WorkOrder, WorkOrderDto>().ReverseMap();
            CreateMap<WorkOrder, WorkOrderDetailDto>().ReverseMap();
            CreateMap<WorkOrder, RequestCreateWorkOrderDto>().ReverseMap();

            CreateMap<VehicleWarrantyPolicy, VehicleWarrantyPolicyDto>().ReverseMap();

            CreateMap<WarrantyPolicy, WarrantyPolicyDto>().ReverseMap();
            CreateMap<WarrantyPolicy, VehicleWarrantyPolicy>().ReverseMap();
            CreateMap<WarrantyPolicy, WarrantyPolicyCreateDto>().ReverseMap();
            CreateMap<WarrantyPolicy, WarrantyPolicyUpdateDto>().ReverseMap();

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

            CreateMap<Organization, OrganizationDto>().ReverseMap();

            CreateMap<Customer, CustomerDto>().ReverseMap();

            CreateMap<Appointment, ResponseAppointmentDto>().ReverseMap();
            CreateMap<Appointment, CreateAppointmentDto>().ReverseMap();
            CreateMap<Appointment, AppointmentDto>().ReverseMap();


            CreateMap<Customer, CustomerDto>().ReverseMap();

            CreateMap<CampaignVehicle, CampaignVehicleDto>()
                .ForMember(d => d.Vehicle, opt => opt.MapFrom(src => src.Vehicle))
                .ForMember(d => d.Customer, opt => opt.MapFrom(src => src.Vehicle != null ? src.Vehicle.Customer : null))
                .ForMember(d => d.NewSerials, opt => opt.MapFrom(src => ParseNewSerials(src.NewSerial)))
                .ForMember(d => d.Replacements, opt => opt.MapFrom(src => src.Replacements.Select(r => new SerialReplacementDto { OldSerial = r.OldSerial, NewSerial = r.NewSerial }).ToList()))
                // map campaign fields
                .ForMember(d => d.Title, opt => opt.MapFrom(src => src.Campaign != null ? src.Campaign.Title : null))
                .ForMember(d => d.Description, opt => opt.MapFrom(src => src.Campaign != null ? src.Campaign.Description : null))
                .ForMember(d => d.Type, opt => opt.MapFrom(src => src.Campaign != null ? src.Campaign.Type : null))
                .ForMember(d => d.StartDate, opt => opt.MapFrom(src => src.Campaign != null ? (DateOnly?)src.Campaign.StartDate : null))
                .ForMember(d => d.EndDate, opt => opt.MapFrom(src => src.Campaign != null ? (DateOnly?)src.Campaign.EndDate : null))
                .ReverseMap()
                .ForMember(s => s.NewSerial, opt => opt.MapFrom(d => SerializeNewSerials(d.NewSerials)));

            // map history
            CreateMap<VehiclePartHistory, VehiclePartHistoryDto>().ReverseMap();
            CreateMap<VehiclePartHistory, ResponseVehiclePartHistoryDto>().ReverseMap();

            // vehicle with history composite
            CreateMap<Vehicle, VehicleWithHistoryDto>();
        }

        private static List<string>? ParseNewSerials(string? newSerialField)
        {
            if (string.IsNullOrWhiteSpace(newSerialField)) return null;

            var trimmed = newSerialField.TrimStart();
            if (trimmed.StartsWith("[") )
            {
                try
                {
                    // Use explicit overload to avoid optional args in expression trees
                    return JsonSerializer.Deserialize<List<string>>(trimmed, (JsonSerializerOptions?)null) ?? new List<string>();
                }
                catch
                {
                    return new List<string>();
                }
            }

            return new List<string> { newSerialField };
        }

        private static string? SerializeNewSerials(List<string>? list)
        {
            if (list == null) return null;
            return JsonSerializer.Serialize(list, (JsonSerializerOptions?)null);
        }
    }
}
