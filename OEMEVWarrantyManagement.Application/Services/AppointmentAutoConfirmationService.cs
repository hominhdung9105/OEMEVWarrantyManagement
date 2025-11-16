using Microsoft.Extensions.Logging;
using OEMEVWarrantyManagement.Application.IRepository;
using OEMEVWarrantyManagement.Application.IServices;
using OEMEVWarrantyManagement.Share.Enums;

namespace OEMEVWarrantyManagement.Application.Services
{
    /// <summary>
    /// Service for handling automatic appointment confirmation timeout
    /// If appointment is not confirmed within 30 minutes, it will be marked as NotConfirmed
    /// </summary>
    public class AppointmentAutoConfirmationService : IAppointmentAutoConfirmationService
    {
        private readonly IAppointmentRepository _appointmentRepository;
        private readonly ILogger<AppointmentAutoConfirmationService> _logger;

        public AppointmentAutoConfirmationService(
            IAppointmentRepository appointmentRepository,
            ILogger<AppointmentAutoConfirmationService> logger)
        {
            _appointmentRepository = appointmentRepository;
            _logger = logger;
        }

        /// <summary>
        /// Check and update appointment status to NotConfirmed if still in Pending status after timeout
        /// </summary>
        /// <param name="appointmentId">The appointment ID to check</param>
        public async Task CheckAndUpdateAppointmentStatusAsync(Guid appointmentId)
        {
            try
            {
                _logger.LogInformation("Checking appointment {AppointmentId} for auto-confirmation timeout", appointmentId);

                var appointment = await _appointmentRepository.GetAppointmentByIdAsync(appointmentId);
                
                if (appointment == null)
                {
                    _logger.LogWarning("Appointment {AppointmentId} not found", appointmentId);
                    return;
                }

                // Only update if still in Pending status
                var pendingStatus = AppointmentStatus.Pending.GetAppointmentStatus();
                if (string.Equals(appointment.Status, pendingStatus, StringComparison.OrdinalIgnoreCase))
                {
                    // Change status to NotConfirmed
                    appointment.Status = AppointmentStatus.NotConfirmed.GetAppointmentStatus();
                    await _appointmentRepository.UpdateAsync(appointment);

                    _logger.LogInformation(
                        "Appointment {AppointmentId} status changed to NotConfirmed due to confirmation timeout", 
                        appointmentId);
                }
                else
                {
                    _logger.LogInformation(
                        "Appointment {AppointmentId} has status {Status}, no action taken", 
                        appointmentId, 
                        appointment.Status);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, 
                    "Error occurred while processing auto-confirmation for appointment {AppointmentId}", 
                    appointmentId);
                throw;
            }
        }
    }
}
