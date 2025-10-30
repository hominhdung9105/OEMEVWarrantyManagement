using OEMEVWarrantyManagement.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OEMEVWarrantyManagement.Application.IRepository
{
    public interface IAppointmentRepository
    {
        Task<IEnumerable<Appointment>> GetAppoinmentByOrgIdAndDateAsync(Guid orgId, DateOnly desiredDate);
        Task<Appointment> CreateAsync(Appointment appointment);
        Task<Appointment> UpdateAsync(Appointment appointment);
        Task<Appointment> GetAppointmentByIdAsync(Guid appointmentId);
        Task<(IEnumerable<Appointment> Data, int TotalRecords)> GetPagedAsync(int pageNumber, int pageSize);
    }
}
