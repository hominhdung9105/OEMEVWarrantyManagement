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
    }
}
