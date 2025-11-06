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
using OEMEVWarrantyManagement.Share.Configs;
using Microsoft.Extensions.Options;
using System.Security.Cryptography;

namespace OEMEVWarrantyManagement.Application.Services
{
    public class AppointmentService : IAppointmentService
    {
        private readonly IAppointmentRepository _appointmentRepository;
        private readonly IVehicleRepository _vehicleRepository;
        private readonly IMapper _mapper;
        private readonly ICurrentUserService _currentUserService;
        private readonly ICustomerRepository _customerRepository;
        private readonly IEmailService _emailService;
        private readonly AppSettings _appSettings;

        public AppointmentService(IAppointmentRepository appointmentRepository, IVehicleRepository vehicleRepository, IMapper mapper, ICurrentUserService currentUserService, ICustomerRepository customerRepository, IEmailService emailService, IOptions<AppSettings> appSettings)
        {
            _appointmentRepository = appointmentRepository;
            _vehicleRepository = vehicleRepository;
            _mapper = mapper;
            _currentUserService = currentUserService;
            _customerRepository = customerRepository;
            _emailService = emailService;
            _appSettings = appSettings.Value;
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
                .ToList();

            // A slot is available if current bookings < SlotCapacity
            var available = allSlots
                .Where(s => !bookedCounts.TryGetValue(s.Slot, out var count) || count < SlotCapacity)
                .Select(s => new AvailableTimeslotDto
                {
                    Slot = s.Slot,
                    Time = s.Time
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

            // default to Scheduled if not provided
            if (string.IsNullOrWhiteSpace(request.Status))
            {
                request.Status = AppointmentStatus.Pending.GetAppointmentStatus();
            }

            var create = _mapper.Map<Appointment>(request);
            var createdAppointment = await _appointmentRepository.CreateAsync(create);

            // Send email to customer to confirm
            await TrySendAppointmentConfirmationEmailAsync(createdAppointment);

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

            request.Status = request.ServiceCenterId == creatorOrgId ? AppointmentStatus.Scheduled.GetAppointmentStatus() : AppointmentStatus.Pending.GetAppointmentStatus();
            request.CreatedAt = DateTime.UtcNow;

            var entity = _mapper.Map<Appointment>(request);
            var created = await _appointmentRepository.CreateAsync(entity);

            // Send email for confirmation if status is Pending (customer needs to confirm)
            await TrySendAppointmentConfirmationEmailAsync(created);

            return _mapper.Map<ResponseAppointmentDto>(created);
        }

        public async Task<AppointmentDto> SubmitAppointmentAsync(Guid appointmentId)
        {
            var entity = await _appointmentRepository.GetAppointmentByIdAsync(appointmentId);
            if (entity == null)
            {
                throw new ApiException(ResponseError.NotFoundAppointment);
            }
            entity.Status = AppointmentStatus.Scheduled.GetAppointmentStatus();
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

        // New: Generic status update
        public async Task<AppointmentDto> UpdateStatusAsync(Guid appointmentId, string status)
        {
            var entity = await _appointmentRepository.GetAppointmentByIdAsync(appointmentId) ?? throw new ApiException(ResponseError.NotFoundAppointment);
            entity.Status = status;
            var updated = await _appointmentRepository.UpdateAsync(entity);
            return _mapper.Map<AppointmentDto>(updated);
        }

        // New: Reschedule with availability check
        public async Task<AppointmentDto> RescheduleAsync(Guid appointmentId, DateOnly newDate, string newSlot)
        {
            var entity = await _appointmentRepository.GetAppointmentByIdAsync(appointmentId) ?? throw new ApiException(ResponseError.NotFoundAppointment);

            // validate slot
            if (TimeSlotExtensions.FromString(newSlot) is null)
                throw new ApiException(ResponseError.InvalidJsonFormat);

            // only allow reschedule from Scheduled state
            if (!string.Equals(entity.Status, AppointmentStatus.Scheduled.GetAppointmentStatus(), StringComparison.OrdinalIgnoreCase))
                throw new ApiException(ResponseError.InvalidJsonFormat);

            var available = await GetAvailableTimeslotAsync(entity.ServiceCenterId, newDate);
            var isAvailable = available.Any(s => string.Equals(s.Slot, newSlot, StringComparison.OrdinalIgnoreCase));
            if (!isAvailable)
                throw new ApiException(ResponseError.InvalidJsonFormat);

            entity.AppointmentDate = newDate;
            entity.Slot = newSlot;

            var updated = await _appointmentRepository.UpdateAsync(entity);
            return _mapper.Map<AppointmentDto>(updated);
        }

        // Generate HMAC token based on appointment info
        private string GenerateConfirmationToken(Appointment appointment)
        {
            var payload = $"{appointment.AppointmentId}|{appointment.Vin}|{appointment.AppointmentDate:yyyyMMdd}|{appointment.Slot}";
            var keyBytes = Encoding.UTF8.GetBytes(_appSettings.Token ?? string.Empty);
            using var hmac = new HMACSHA256(keyBytes);
            var hash = hmac.ComputeHash(Encoding.UTF8.GetBytes(payload));
            return Convert.ToBase64String(hash)
                .TrimEnd('=')
                .Replace('+', '-')
                .Replace('/', '_'); // URL-safe
        }

        private bool ValidateConfirmationToken(Appointment appointment, string token)
        {
            var expected = GenerateConfirmationToken(appointment);
            return SlowEquals(expected, token);
        }

        private static bool SlowEquals(string a, string b)
        {
            var aBytes = Encoding.UTF8.GetBytes(a ?? string.Empty);
            var bBytes = Encoding.UTF8.GetBytes(b ?? string.Empty);
            uint diff = (uint)aBytes.Length ^ (uint)bBytes.Length;
            for (int i = 0; i < aBytes.Length && i < bBytes.Length; i++)
            {
                diff |= (uint)(aBytes[i] ^ bBytes[i]);
            }
            return diff == 0;
        }

        private async Task TrySendAppointmentConfirmationEmailAsync(Appointment appointment)
        {
            // Only send email when status is Pending (requires customer confirmation)
            if (!string.Equals(appointment.Status, AppointmentStatus.Pending.GetAppointmentStatus(), StringComparison.OrdinalIgnoreCase))
                return;

            // Get customer by VIN
            var vehicle = await _vehicleRepository.GetVehicleByVinAsync(appointment.Vin);
            if (vehicle == null) return;

            var customer = await _customerRepository.GetCustomerByIdAsync(vehicle.CustomerId);
            if (customer == null || string.IsNullOrWhiteSpace(customer.Email)) return;

            var token = GenerateConfirmationToken(appointment);
            var confirmUrl = $"{_appSettings.Issuer?.TrimEnd('/')}/api/appointment/confirm?appointmentId={appointment.AppointmentId}&token={token}";

            var slotInfo = TimeSlotExtensions.GetSlotInfo(appointment.Slot);
            var time = slotInfo?.Time ?? appointment.Slot;

            try
            {
                await _emailService.SendAppointmentConfirmationEmailAsync(
                    to: customer.Email,
                    customerName: customer.Name,
                    vin: appointment.Vin,
                    date: appointment.AppointmentDate,
                    slot: appointment.Slot,
                    time: time,
                    confirmUrl: confirmUrl
                );
            }
            catch
            {
                // log handled in EmailService; swallow to not block booking
            }
        }

        public async Task<bool> ConfirmAppointmentAsync(Guid appointmentId, string token)
        {
            var entity = await _appointmentRepository.GetAppointmentByIdAsync(appointmentId) ?? throw new ApiException(ResponseError.NotFoundAppointment);

            // Only allow confirmation from Pending -> Scheduled
            if (!string.Equals(entity.Status, AppointmentStatus.Pending.GetAppointmentStatus(), StringComparison.OrdinalIgnoreCase))
                return false;

            if (!ValidateConfirmationToken(entity, token))
                return false;

            entity.Status = AppointmentStatus.Scheduled.GetAppointmentStatus();
            await _appointmentRepository.UpdateAsync(entity);
            return true;
        }
    }
}
