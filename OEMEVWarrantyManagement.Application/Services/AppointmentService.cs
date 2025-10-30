using AutoMapper;
using OEMEVWarrantyManagement.Application.Dtos;
using OEMEVWarrantyManagement.Application.IRepository;
using OEMEVWarrantyManagement.Application.IServices;
using OEMEVWarrantyManagement.Domain.Entities;
using OEMEVWarrantyManagement.Share.Enums;
using OEMEVWarrantyManagement.Share.Exceptions;
using OEMEVWarrantyManagement.Share.Models.Pagination;
using OEMEVWarrantyManagement.Share.Models.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OEMEVWarrantyManagement.Application.Services
{
    public class AppointmentService : IAppointmentService
    {
        private readonly IAppointmentRepository _appointmentRepository;
        private readonly IVehicleRepository _vehicleRepository;
        private readonly IMapper _mapper;

        public AppointmentService(IAppointmentRepository appointmentRepository, IVehicleRepository vehicleRepository, IMapper mapper)
        {
            _appointmentRepository = appointmentRepository;
            _vehicleRepository = vehicleRepository;
            _mapper = mapper;
        }

        public async Task<IEnumerable<AvailableTimeslotDto>> GetAvailableTimeslotAsync(Guid orgId, DateOnly desiredDate)
        {
            var appointments = await _appointmentRepository.GetAppoinmentByOrgIdAndDateAsync(orgId, desiredDate);

            var bookedSlots = appointments
                .Select(a => a.Slot)
                .ToList();

            var allSlots = TimeSlotExtensions.GetAllSlots()
                .Select(s => new { slot = (string)s.slot, time = (string)s.time })
                .ToList();

            var available = allSlots
                .Where(s => !bookedSlots.Contains(s.slot))
                .Select(s => new AvailableTimeslotDto
                {
                    Slot = s.slot,
                    Time = s.time
                });

            return available;
        }

        public async Task<ResponseAppointmentDto> CreateAppointmentAsync(CreateAppointmentDto request)
        {
            var vehicle = await _vehicleRepository.GetVehicleByVinAsync(request.Vin);
            if (vehicle == null)
            {
                throw new ApiException(ResponseError.NotfoundVin);
            }
            request.CustomerId = vehicle.CustomerId;
            request.Status = "Pending";
            request.CreatedAt = DateTime.Now;
            var create = _mapper.Map<Appointment>(request);
            var createdAppointment = await _appointmentRepository.CreateAsync(create);
            var response = _mapper.Map<ResponseAppointmentDto>(createdAppointment);
            return response;
        }

        public async Task<AppointmentDto> SubmitAppointmentAsync(Guid appointmentId)
        {
            var entity = await _appointmentRepository.GetAppointmentByIdAsync(appointmentId);
            if (entity == null)
            {
                throw new ApiException(ResponseError.NotFoundAppointment);
            }
            entity.Status = "Scheduled";
            var updatedAppointment = await _appointmentRepository.UpdateAsync(entity);
            return _mapper.Map<AppointmentDto>(updatedAppointment);
        }

        public async Task<PagedResult<AppointmentDto>> GetPagedAsync(PaginationRequest request)
        {
            var (entities, totalRecords) = await _appointmentRepository.GetPagedAsync(request.Page, request.Size);
            var totalPages = (int)Math.Ceiling(totalRecords / (double)request.Size);
            var results = _mapper.Map<IEnumerable<AppointmentDto>>(entities);

            return new PagedResult<AppointmentDto>
            {
                PageNumber = request.Page,
                PageSize = request.Size,
                TotalRecords = totalRecords,
                TotalPages = totalPages,
                Items = results
            };
        }
    }
}
