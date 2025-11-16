namespace OEMEVWarrantyManagement.Application.IServices
{
    public interface IAppointmentAutoConfirmationService
    {
        Task CheckAndUpdateAppointmentStatusAsync(Guid appointmentId);
    }
}
