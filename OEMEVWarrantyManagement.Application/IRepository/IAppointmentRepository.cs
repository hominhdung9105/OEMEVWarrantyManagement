using OEMEVWarrantyManagement.Domain.Entities;

namespace OEMEVWarrantyManagement.Application.IRepository
{
    public interface IAppointmentRepository
    {
        Task<IEnumerable<Appointment>> GetAppoinmentByOrgIdAndDateAsync(Guid orgId, DateOnly desiredDate);
        Task<Appointment> CreateAsync(Appointment appointment);
        Task<Appointment> UpdateAsync(Appointment appointment);
        Task<Appointment> GetAppointmentByIdAsync(Guid appointmentId);
        Task<(IEnumerable<Appointment> Data, int TotalRecords)> GetPagedAsync(int pageNumber, int pageSize);
        Task<int> CountByOrgIdAndStatusAsync(Guid orgId, string status);
        Task<int> CountByTypeAndStatusAsync(string appointmentType, string status);
        Task<int> CountByStatusAsync(string status);
        Task<List<Appointment>> GetAppointmentsByVinAsync(string vin);
    }
}
