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
        private readonly ICurrentUserService _currentUserService;
        private readonly ICustomerRepository _customerRepository;


        public AppointmentService(IAppointmentRepository appointmentRepository, IVehicleRepository vehicleRepository, IMapper mapper, ICurrentUserService currentUserService, ICustomerRepository customerRepository)
        {
            _appointmentRepository = appointmentRepository;
            _vehicleRepository = vehicleRepository;
            _mapper = mapper;
            _currentUserService = currentUserService;
            _customerRepository = customerRepository;
        }

        public async Task<IEnumerable<AvailableTimeslotDto>> GetAvailableTimeslotAsync(Guid orgId, DateOnly desiredDate)
        {
            const int SlotCapacity = 2;
            var appointments = await _appointmentRepository.GetAppoinmentByOrgIdAndDateAsync(orgId, desiredDate);

            // Count existing bookings per slot
            var bookedCounts = appointments
                .GroupBy(a => a.Slot)
                .ToDictionary(g => g.Key, g => g.Count());

            var allSlots = TimeSlotExtensions.GetAllSlots()
                .Select(s => new { slot = (string)s.slot, time = (string)s.time })
                .ToList();

            // A slot is available if current bookings < SlotCapacity
            var available = allSlots
                .Where(s => !bookedCounts.TryGetValue(s.slot, out var count) || count < SlotCapacity)
                .Select(s => new AvailableTimeslotDto
                {
                    Slot = s.slot,
                    Time = s.time
                });

            return available;
        }

        public async Task<ResponseAppointmentDto> CreateAppointmentAsync(CreateAppointmentDto request)
        {
            // Validate: chỉ cho đặt sau 3 ngày kể từ hôm nay (UTC)
            var today = DateOnly.FromDateTime(DateTime.UtcNow.Date);
            var minDate = today.AddDays(3);
            if (request.AppointmentDate < minDate)
            {
                throw new ApiException(ResponseError.InvalidJsonFormat);
            }

            // Validate: Slot hợp lệ theo TimeSlotEnum
            if (TimeSlotExtensions.FromString(request.Slot) is null)
            {
                throw new ApiException(ResponseError.InvalidJsonFormat);
            }

            // Validate vehicle exists by VIN
            var vehicle = await _vehicleRepository.GetVehicleByVinAsync(request.Vin);
            if (vehicle == null)
            {
                throw new ApiException(ResponseError.NotfoundVin);
            }

            request.CreatedAt = DateTime.UtcNow;

            // Re-check availability with capacity considered
            var available = await GetAvailableTimeslotAsync(request.ServiceCenterId, request.AppointmentDate);
            var isAvailable = available.Any(s => string.Equals(s.Slot, request.Slot, StringComparison.OrdinalIgnoreCase));
            if (!isAvailable)
            {
                throw new ApiException(ResponseError.InvalidJsonFormat);
            }

            var create = _mapper.Map<Appointment>(request);
            var createdAppointment = await _appointmentRepository.CreateAsync(create);
            var response = _mapper.Map<ResponseAppointmentDto>(createdAppointment);
            return response;
        }

        public async Task<ResponseAppointmentDto> CreateAppointmentByEvmAsync(CreateAppointmentDto request)
        {
            // Validate: chỉ cho đặt sau 3 ngày kể từ hôm nay (UTC)
            var today = DateOnly.FromDateTime(DateTime.UtcNow.Date);
            var minDate = today.AddDays(3);
            if (request.AppointmentDate < minDate)
            {
                throw new ApiException(ResponseError.InvalidJsonFormat);
            }

            if (TimeSlotExtensions.FromString(request.Slot) is null)
            {
                throw new ApiException(ResponseError.InvalidJsonFormat);
            }

            var vehicle = await _vehicleRepository.GetVehicleByVinAsync(request.Vin) ?? throw new ApiException(ResponseError.NotfoundVin);
            var creatorOrgId = await _currentUserService.GetOrgId();

            // Re-check availability with capacity considered
            var available = await GetAvailableTimeslotAsync(request.ServiceCenterId, request.AppointmentDate);
            var isAvailable = available.Any(s => string.Equals(s.Slot, request.Slot, StringComparison.OrdinalIgnoreCase));
            if (!isAvailable)
            {
                throw new ApiException(ResponseError.InvalidJsonFormat);
            }

            request.Status = request.ServiceCenterId == creatorOrgId ? "Scheduled" : "Pending";
            request.CreatedAt = DateTime.UtcNow;

            var entity = _mapper.Map<Appointment>(request);
            var created = await _appointmentRepository.CreateAsync(entity);
            return _mapper.Map<ResponseAppointmentDto>(created);
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
            
            // Populate customer information from Vehicle
            foreach (var appointment in results)
            {
                var entity = entities.FirstOrDefault(e => e.AppointmentId == appointment.AppointmentId);
                if (entity?.Vehicle?.Customer != null)
                {
                    var customer = entity.Vehicle.Customer;
                    appointment.CustomerName = customer.Name;
                    appointment.CustomerPhoneNumber = customer.Phone;
                    appointment.CustomerEmail = customer.Email;
                }
            }

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
