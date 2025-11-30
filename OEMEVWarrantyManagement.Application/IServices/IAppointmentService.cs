using OEMEVWarrantyManagement.Application.Dtos;
using OEMEVWarrantyManagement.Share.Models.Pagination;

namespace OEMEVWarrantyManagement.Application.IServices
{
    public interface IAppointmentService
    {
        Task<IEnumerable<AvailableTimeslotDto>> GetAvailableTimeslotAsync(Guid orgId, DateOnly desiredDate);
        Task<ResponseAppointmentDto> CreateAppointmentAsync(CreateAppointmentDto request);
        Task<ResponseAppointmentDto> CreateAppointmentByEvmAsync(CreateAppointmentDto request);
        Task<AppointmentDto> SubmitAppointmentAsync(Guid appointmentId);
        Task<PagedResult<AppointmentDto>> GetPagedAsync(PaginationRequest request);
        Task<AppointmentDto> UpdateStatusAsync(Guid appointmentId, string status);
        Task<AppointmentDto> RescheduleAsync(Guid appointmentId, DateOnly newDate, string newSlot);
        Task<ConfirmAppointmentResponseDto?> ConfirmAppointmentAsync(Guid appointmentId, string token);
    }
}
