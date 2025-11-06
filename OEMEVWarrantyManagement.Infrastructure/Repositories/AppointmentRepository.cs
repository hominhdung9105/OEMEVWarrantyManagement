using Microsoft.EntityFrameworkCore;
using OEMEVWarrantyManagement.Application.IRepository;
using OEMEVWarrantyManagement.Domain.Entities;
using OEMEVWarrantyManagement.Infrastructure.Persistence;
using OEMEVWarrantyManagement.Share.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OEMEVWarrantyManagement.Infrastructure.Repositories
{
    public class AppointmentRepository : IAppointmentRepository
    {
        private readonly AppDbContext _context;
        public AppointmentRepository(AppDbContext context)
        {
            _context = context;
        }
        public async Task<IEnumerable<Appointment>> GetAppoinmentByOrgIdAndDateAsync(Guid orgId, DateOnly desiredDate)
        {
            var scheduled = AppointmentStatus.Scheduled.GetAppointmentStatus();
            var checkedIn = AppointmentStatus.CheckedIn.GetAppointmentStatus();

            var entity = await _context.Appointments
                .Where(a => a.ServiceCenterId == orgId
                         && a.AppointmentDate == desiredDate
                         && (a.Status == scheduled || a.Status == checkedIn))
                .OrderBy(a => a.Slot)
                .ToListAsync();
            return entity;
        }

        public async Task<Appointment> CreateAsync(Appointment appointment)
        {
            await _context.Appointments.AddAsync(appointment);
            await _context.SaveChangesAsync();
            return appointment;
        }
        public async Task<Appointment> UpdateAsync(Appointment appointment)
        {
            _context.Appointments.Update(appointment);
            await _context.SaveChangesAsync();
            return appointment;
        }
        public async Task<Appointment> GetAppointmentByIdAsync(Guid appointmentId)
        {
            return await _context.Appointments.FirstOrDefaultAsync(a => a.AppointmentId == appointmentId);
        }

        public async Task<(IEnumerable<Appointment> Data, int TotalRecords)> GetPagedAsync(int pageNumber, int pageSize)
        {
            var query = _context.Appointments
                .Include(a => a.Vehicle)
                    .ThenInclude(v => v.Customer)
                .AsQueryable();
            
            var totalRecords = await query.CountAsync();

            var data = await query
                .OrderByDescending(a => a.CreatedAt)
                .Skip(pageNumber * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return (data, totalRecords);
        }

        public async Task<int> CountByOrgIdAndStatusAsync(Guid orgId, string status)
        {
            return await _context.Appointments
                .Where(a => a.ServiceCenterId == orgId && a.Status == status)
                .CountAsync();
        }
    }
}
