using OEMEVWarrantyManagement.Application.Dtos;
using OEMEVWarrantyManagement.Domain.Entities;
using OEMEVWarrantyManagement.Share.Models.Pagination;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OEMEVWarrantyManagement.Application.IServices
{
    public interface IAppointmentService
    {
        Task<IEnumerable<AvailableTimeslotDto>> GetAvailableTimeslotAsync(Guid orgId, DateOnly desiredDate);
        Task<ResponseAppointmentDto> CreateAppointmentAsync(CreateAppointmentDto request);
        Task<ResponseAppointmentDto> CreateAppointmentByEvmAsync(CreateAppointmentDto request);
        Task<AppointmentDto> SubmitAppointmentAsync(Guid appointmentId);
        Task<PagedResult<AppointmentDto>> GetPagedAsync(PaginationRequest request);
        // New
        Task<AppointmentDto> UpdateStatusAsync(Guid appointmentId, string status);
        Task<AppointmentDto> RescheduleAsync(Guid appointmentId, DateOnly newDate, string newSlot);
        // Updated: Customer confirmation returns detailed info
        Task<ConfirmAppointmentResponseDto?> ConfirmAppointmentAsync(Guid appointmentId, string token);
    }
}
