using OEMEVWarrantyManagement.Application.IRepository;
using OEMEVWarrantyManagement.Share.Enums;

namespace OEMEVWarrantyManagement.Application.BackgroundJobs
{
    /// <summary>
    /// Background job to automatically cancel unconfirmed appointments
    /// </summary>
    public class AppointmentCancellationJob
    {
        private readonly IAppointmentRepository _appointmentRepository;

        public AppointmentCancellationJob(IAppointmentRepository appointmentRepository)
        {
            _appointmentRepository = appointmentRepository;
        }

        /// <summary>
        /// Cancels an appointment if it's still in Pending status (not confirmed)
        /// </summary>
        public async Task CancelUnconfirmedAppointmentAsync(Guid appointmentId)
        {
            var appointment = await _appointmentRepository.GetAppointmentByIdAsync(appointmentId);
            
            if (appointment == null)
            {
                // Appointment not found, nothing to do
                return;
            }

            // Only cancel if still in Pending status
            if (string.Equals(appointment.Status, AppointmentStatus.Pending.GetAppointmentStatus(), StringComparison.OrdinalIgnoreCase))
            {
                appointment.Status = AppointmentStatus.NotConfirmed.GetAppointmentStatus();
                await _appointmentRepository.UpdateAsync(appointment);
            }
        }
    }
}
