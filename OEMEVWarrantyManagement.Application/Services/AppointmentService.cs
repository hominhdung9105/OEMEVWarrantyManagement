using AutoMapper;
using OEMEVWarrantyManagement.Application.Dtos;
using OEMEVWarrantyManagement.Application.IRepository;
using OEMEVWarrantyManagement.Application.IServices;
using OEMEVWarrantyManagement.Domain.Entities;
using OEMEVWarrantyManagement.Share.Enums;
using OEMEVWarrantyManagement.Share.Exceptions;
using OEMEVWarrantyManagement.Share.Models.Pagination;
using OEMEVWarrantyManagement.Share.Models.Response;
using System.Text;
using OEMEVWarrantyManagement.Share.Configs;
using Microsoft.Extensions.Options;
using System.Security.Cryptography;
using Hangfire;
using OEMEVWarrantyManagement.Application.BackgroundJobs;

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
        private readonly EmailUrlSettings _emailUrlSettings;
        private readonly IBackgroundJobClient _backgroundJobClient;

        public AppointmentService(
            IAppointmentRepository appointmentRepository, 
            IVehicleRepository vehicleRepository, 
            IMapper mapper, 
            ICurrentUserService currentUserService, 
            ICustomerRepository customerRepository, 
            IEmailService emailService, 
            IOptions<AppSettings> appSettings,
            IOptions<EmailUrlSettings> emailUrlSettings,
            IBackgroundJobClient backgroundJobClient)
        {
            _appointmentRepository = appointmentRepository;
            _vehicleRepository = vehicleRepository;
            _mapper = mapper;
            _currentUserService = currentUserService;
            _customerRepository = customerRepository;
            _emailService = emailService;
            _appSettings = appSettings.Value;
            _emailUrlSettings = emailUrlSettings.Value;
            _backgroundJobClient = backgroundJobClient;
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

            // Schedule Hangfire job to cancel appointment after 30 minutes if not confirmed
            if (string.Equals(createdAppointment.Status, AppointmentStatus.Pending.GetAppointmentStatus(), StringComparison.OrdinalIgnoreCase))
            {
                var jobId = _backgroundJobClient.Schedule<AppointmentCancellationJob>(
                    job => job.CancelUnconfirmedAppointmentAsync(createdAppointment.AppointmentId),
                    TimeSpan.FromMinutes(30)
                );
                
                // Store job ID in appointment note for later cancellation if needed
                // You could also add a JobId property to Appointment entity if preferred
                createdAppointment.Note = string.IsNullOrEmpty(createdAppointment.Note) 
                    ? $"HangfireJobId:{jobId}" 
                    : $"{createdAppointment.Note}|HangfireJobId:{jobId}";
                await _appointmentRepository.UpdateAsync(createdAppointment);
            }

            // Mask email before returning
            var customer = await _customerRepository.GetCustomerByIdAsync(vehicle.CustomerId);
            var email = customer?.Email;
            if (!string.IsNullOrEmpty(email) && email.Contains("@"))
            {
                var parts = email.Split('@');
                var user = parts[0];
                var maskedUser = user.Length > 3 ? user.Substring(0, 3) + "****" : "****";
                email = maskedUser + "@" + parts[1];
            }


            // Send email to customer to confirm
            await TrySendAppointmentConfirmationEmailAsync(createdAppointment);

            var response = _mapper.Map<ResponseAppointmentDto>(createdAppointment);
            response.Email = email;
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

            // Schedule Hangfire job to cancel appointment after 30 minutes if status is Pending
            if (string.Equals(created.Status, AppointmentStatus.Pending.GetAppointmentStatus(), StringComparison.OrdinalIgnoreCase))
            {
                var jobId = _backgroundJobClient.Schedule<AppointmentCancellationJob>(
                    job => job.CancelUnconfirmedAppointmentAsync(created.AppointmentId),
                    TimeSpan.FromMinutes(30)
                );
                
                created.Note = string.IsNullOrEmpty(created.Note) 
                    ? $"HangfireJobId:{jobId}" 
                    : $"{created.Note}|HangfireJobId:{jobId}";
                await _appointmentRepository.UpdateAsync(created);
            }

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
            
            // Populate customer information and vehicle information from Vehicle
            foreach (var appointment in results)
            {
                var entity = entities.FirstOrDefault(e => e.AppointmentId == appointment.AppointmentId);
                if (entity?.Vehicle != null)
                {
                    // Vehicle information
                    appointment.Model = entity.Vehicle.Model;
                    appointment.Year = entity.Vehicle.Year;
                    
                    // Customer information
                    if (entity.Vehicle.Customer != null)
                    {
                        appointment.CustomerName = entity.Vehicle.Customer.Name;
                        appointment.CustomerPhoneNumber = entity.Vehicle.Customer.Phone;
                        appointment.CustomerEmail = entity.Vehicle.Customer.Email;
                    }
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

        // New: Generic status update with email notification
        public async Task<AppointmentDto> UpdateStatusAsync(Guid appointmentId, string status)
        {
            var entity = await _appointmentRepository.GetAppointmentByIdAsync(appointmentId) ?? throw new ApiException(ResponseError.NotFoundAppointment);
            
            // Store old status for comparison
            var oldStatus = entity.Status;
            
            entity.Status = status;
            var updated = await _appointmentRepository.UpdateAsync(entity);
            
            // Send email notification for specific status changes
            await TrySendStatusChangeEmailAsync(updated, oldStatus, status);
            
            return _mapper.Map<AppointmentDto>(updated);
        }

        // New: Reschedule with availability check and email notification
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

            // Store old date and slot for email
            var oldDate = entity.AppointmentDate;
            var oldSlot = entity.Slot;

            entity.AppointmentDate = newDate;
            entity.Slot = newSlot;
            entity.Status = AppointmentStatus.Pending.GetAppointmentStatus(); // Change status back to Pending

            var updated = await _appointmentRepository.UpdateAsync(entity);

            // Schedule new Hangfire job for the rescheduled appointment
            var jobId = _backgroundJobClient.Schedule<AppointmentCancellationJob>(
                job => job.CancelUnconfirmedAppointmentAsync(updated.AppointmentId),
                TimeSpan.FromMinutes(30)
            );
            
            updated.Note = string.IsNullOrEmpty(updated.Note) 
                ? $"HangfireJobId:{jobId}" 
                : $"{updated.Note}|HangfireJobId:{jobId}";
            await _appointmentRepository.UpdateAsync(updated);
            
            // Send reschedule email with new confirmation link
            await TrySendRescheduleEmailAsync(updated, oldDate, oldSlot);
            
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
                diff |= (uint)(aBytes[i] ^ (uint)bBytes[i]);
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
            
            // Use EmailUrlSettings for appointment confirmation URL
            var baseUrl = _emailUrlSettings.AppointmentConfirmationUrl?.TrimEnd('/');
            if (string.IsNullOrWhiteSpace(baseUrl))
            {
                return;
            }
            
            var confirmUrl = $"{baseUrl}?appointmentId={appointment.AppointmentId}&token={token}";

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

        public async Task<ConfirmAppointmentResponseDto?> ConfirmAppointmentAsync(Guid appointmentId, string token)
        {
            var entity = await _appointmentRepository.GetAppointmentByIdAsync(appointmentId) ?? throw new ApiException(ResponseError.NotFoundAppointment);

            // Only allow confirmation from Pending -> Scheduled
            if (!string.Equals(entity.Status, AppointmentStatus.Pending.GetAppointmentStatus(), StringComparison.OrdinalIgnoreCase))
                return null;

            if (!ValidateConfirmationToken(entity, token))
                return null;

            // Cancel the scheduled Hangfire job since appointment is confirmed
            var jobId = ExtractJobIdFromNote(entity.Note);
            if (!string.IsNullOrEmpty(jobId))
            {
                _backgroundJobClient.Delete(jobId);
            }

            entity.Status = AppointmentStatus.Scheduled.GetAppointmentStatus();
            await _appointmentRepository.UpdateAsync(entity);

            // Build confirmation response
            var vehicle = await _vehicleRepository.GetVehicleByVinAsync(entity.Vin);
            var customer = vehicle != null ? await _customerRepository.GetCustomerByIdAsync(vehicle.CustomerId) : null;
            var email = customer?.Email;

            var slotInfo = TimeSlotExtensions.GetSlotInfo(entity.Slot);
            var time = slotInfo?.Time ?? entity.Slot;
            var combinedSlot = slotInfo != null ? $"{slotInfo.Slot} - {slotInfo.Time}" : entity.Slot;

            return new ConfirmAppointmentResponseDto
            {
                Vin = entity.Vin,
                AppointmentType = entity.AppointmentType,
                AppointmentDate = entity.AppointmentDate,
                Slot = combinedSlot,
                Time = time,
                Email = email
            };
        }

        // New: Send email notification when appointment status updated
        private async Task TrySendStatusChangeEmailAsync(Appointment appointment, string oldStatus, string newStatus)
        {
            try
            {
                // Get customer information
                var vehicle = await _vehicleRepository.GetVehicleByVinAsync(appointment.Vin);
                if (vehicle == null) return;

                var customer = await _customerRepository.GetCustomerByIdAsync(vehicle.CustomerId);
                if (customer == null || string.IsNullOrWhiteSpace(customer.Email)) return;

                var slotInfo = TimeSlotExtensions.GetSlotInfo(appointment.Slot);
                var time = slotInfo?.Time ?? appointment.Slot;

                // Send email based on new status
                if (string.Equals(newStatus, AppointmentStatus.Cancelled.GetAppointmentStatus(), StringComparison.OrdinalIgnoreCase))
                {
                    await _emailService.SendAppointmentCancelledEmailAsync(
                        to: customer.Email,
                        customerName: customer.Name,
                        vin: appointment.Vin
                    );
                }
                else if (string.Equals(newStatus, AppointmentStatus.NoShow.GetAppointmentStatus(), StringComparison.OrdinalIgnoreCase))
                {
                    await _emailService.SendAppointmentNoShowEmailAsync(
                        to: customer.Email,
                        customerName: customer.Name,
                        vin: appointment.Vin,
                        date: appointment.AppointmentDate,
                        slot: appointment.Slot,
                        time: time
                    );
                }
            }
            catch
            {
                // log handled in EmailService; swallow to not block status update
            }
        }

        // New: Send email notification when appointment is rescheduled
        private async Task TrySendRescheduleEmailAsync(Appointment appointment, DateOnly oldDate, string oldSlot)
        {
            try
            {
                // Get customer information
                var vehicle = await _vehicleRepository.GetVehicleByVinAsync(appointment.Vin);
                if (vehicle == null) return;

                var customer = await _customerRepository.GetCustomerByIdAsync(vehicle.CustomerId);
                if (customer == null || string.IsNullOrWhiteSpace(customer.Email)) return;

                // Generate new confirmation token
                var token = GenerateConfirmationToken(appointment);
                
                // Use EmailUrlSettings for appointment confirmation URL
                var baseUrl = _emailUrlSettings.AppointmentConfirmationUrl?.TrimEnd('/');
                if (string.IsNullOrWhiteSpace(baseUrl))
                {
                    return;
                }
                
                var confirmUrl = $"{baseUrl}?appointmentId={appointment.AppointmentId}&token={token}";

                var slotInfo = TimeSlotExtensions.GetSlotInfo(appointment.Slot);
                var time = slotInfo?.Time ?? appointment.Slot;

                await _emailService.SendAppointmentRescheduledEmailAsync(
                    to: customer.Email,
                    customerName: customer.Name,
                    vin: appointment.Vin,
                    oldDate: oldDate,
                    oldSlot: oldSlot,
                    newDate: appointment.AppointmentDate,
                    newSlot: appointment.Slot,
                    newTime: time,
                    confirmUrl: confirmUrl
                );
            }
            catch
            {
                // log handled in EmailService; swallow to not block reschedule
            }
        }

        /// <summary>
        /// Extract Hangfire job ID from appointment note
        /// </summary>
        private string? ExtractJobIdFromNote(string? note)
        {
            if (string.IsNullOrEmpty(note))
                return null;

            var parts = note.Split('|');
            foreach (var part in parts)
            {
                if (part.StartsWith("HangfireJobId:"))
                {
                    return part.Substring("HangfireJobId:".Length);
                }
            }

            return null;
        }
    }
}
