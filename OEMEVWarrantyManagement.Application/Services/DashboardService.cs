using OEMEVWarrantyManagement.Application.Dtos;
using OEMEVWarrantyManagement.Application.IRepository;
using OEMEVWarrantyManagement.Application.IServices;
using OEMEVWarrantyManagement.Share.Enums;

namespace OEMEVWarrantyManagement.Application.Services
{
    public class DashboardService : IDashboardService
    {
        private readonly ICurrentUserService _currentUserService;
        private readonly IVehicleRepository _vehicleRepository;
        private readonly IAppointmentRepository _appointmentRepository;
        private readonly ICampaignRepository _campaignRepository;
        private readonly IWarrantyClaimRepository _warrantyClaimRepository;
        private readonly IEmployeeRepository _employeeRepository;
        private readonly IWorkOrderRepository _workOrderRepository;

        public DashboardService(
            ICurrentUserService currentUserService,
            IVehicleRepository vehicleRepository,
            IAppointmentRepository appointmentRepository,
            ICampaignRepository campaignRepository,
            IWarrantyClaimRepository warrantyClaimRepository,
            IEmployeeRepository employeeRepository,
            IWorkOrderRepository workOrderRepository)
        {
            _currentUserService = currentUserService;
            _vehicleRepository = vehicleRepository;
            _appointmentRepository = appointmentRepository;
            _campaignRepository = campaignRepository;
            _warrantyClaimRepository = warrantyClaimRepository;
            _employeeRepository = employeeRepository;
            _workOrderRepository = workOrderRepository;
        }

        public async Task<DashboardSummaryDto> GetSummaryAsync()
        {
            var orgId = await _currentUserService.GetOrgId();

            var vehicleCount = await _vehicleRepository.CountByOrgIdAsync(orgId);

            var scheduled = AppointmentStatus.Scheduled.GetAppointmentStatus();
            var scheduledAppointmentCount = await _appointmentRepository.CountByOrgIdAndStatusAsync(orgId, scheduled);

            var activeCampaignCount = await _campaignRepository.CountByStatusAsync("Active");//TODO-ENUM

            var repaired = WarrantyClaimStatus.Repaired.GetWarrantyClaimStatus();
            var repairedWarrantyClaimCount = await _warrantyClaimRepository.CountByOrgIdAndStatusAsync(orgId, repaired);

            // Lấy số lượng warranty claim theo từng tháng trong 6 tháng gần nhất
            var warrantyClaimsByMonth = await _warrantyClaimRepository.CountByOrgIdGroupByMonthAsync(orgId, 6);
            
            // Tạo danh sách đầy đủ 6 tháng (bao gồm cả tháng không có claim)
            var warrantyClaimLastSixMonths = new List<MonthlyWarrantyClaimDto>();
            var now = DateTime.Now;
            
            for (int i = 5; i >= 0; i--)
            {
                var monthDate = now.AddMonths(-i);
                var firstDayOfMonth = new DateTime(monthDate.Year, monthDate.Month, 1);
                
                warrantyClaimsByMonth.TryGetValue(firstDayOfMonth, out int count);
                
                warrantyClaimLastSixMonths.Add(new MonthlyWarrantyClaimDto
                {
                    Year = monthDate.Year,
                    Month = monthDate.Month,
                    MonthName = monthDate.ToString("MMM yyyy"), // e.g., "Jan 2024"
                    Count = count
                });
            }

            // Lấy danh sách các technician trong organization
            var technicians = await _employeeRepository.GetAllTechInWorkspaceAsync(orgId);

            // Đếm số work order cho mỗi technician
            var techWorkOrderCounts = new List<TechWorkOrderCountDto>();
            foreach (var tech in technicians)
            {
                var workOrderCount = await _workOrderRepository.CountByTechIdAsync(tech.UserId);
                techWorkOrderCounts.Add(new TechWorkOrderCountDto
                {
                    TechName = tech.Name,
                    WorkOrderCount = workOrderCount
                });
            }

            return new DashboardSummaryDto
            {
                VehicleCount = vehicleCount,
                ScheduledAppointmentCount = scheduledAppointmentCount,
                ActiveCampaignCount = activeCampaignCount,
                RepairedWarrantyClaimCount = repairedWarrantyClaimCount,
                WarrantyClaimLastSixMonths = warrantyClaimLastSixMonths,
                TechWorkOrderCounts = techWorkOrderCounts
            };
        }
    }
}
