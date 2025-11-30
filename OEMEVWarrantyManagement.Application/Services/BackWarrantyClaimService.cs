using AutoMapper;
using OEMEVWarrantyManagement.Application.Dtos;
using OEMEVWarrantyManagement.Application.IRepository;
using OEMEVWarrantyManagement.Application.IServices;
using OEMEVWarrantyManagement.Domain.Entities;
using OEMEVWarrantyManagement.Share.Enums;
using OEMEVWarrantyManagement.Share.Exceptions;

namespace OEMEVWarrantyManagement.Application.Services
{
    public class BackWarrantyClaimService : IBackWarrantyClaimService
    {
        private readonly IBackWarrantyClaimRepository _backWarrantyClaimRepository;
        private readonly IMapper _mapper;
        private readonly ICurrentUserService _currentUserService;
        private readonly IWarrantyClaimRepository _warrantyClaimRepository;
        private readonly IEmployeeRepository _employeeRepository;
        private readonly IWorkOrderRepository _workOrderRepository;

        public BackWarrantyClaimService(IBackWarrantyClaimRepository backWarrantyClaimRepository, IMapper mapper, ICurrentUserService currentUserService, IWarrantyClaimRepository warrantyClaimRepository, IEmployeeRepository employeeRepository, IWorkOrderRepository workOrderRepository)
        {
            _backWarrantyClaimRepository = backWarrantyClaimRepository;
            _mapper = mapper;
            _currentUserService = currentUserService;
            _warrantyClaimRepository = warrantyClaimRepository;
            _employeeRepository = employeeRepository;
            _workOrderRepository = workOrderRepository;
        }
        public async Task<BackWarrantyClaimDto> CreateBackWarrantyClaimAsync(CreateBackWarrantyClaimRequestDto request)
        {
            var entity = _mapper.Map<BackWarrantyClaim>(request);
            entity.CreatedDate = DateTime.Now;
            entity.CreatedByEmployeeId = _currentUserService.GetUserId();

            // fetch warranty claim
            var warrantyClaim = await _warrantyClaimRepository.GetWarrantyClaimByIdAsync(entity.WarrantyClaimId) ?? throw new ApiException(OEMEVWarrantyManagement.Share.Models.Response.ResponseError.NotFoundWarrantyClaim);

            // get current employee and role
            var currentUserId = _currentUserService.GetUserId();
            var currentEmployee = await _employeeRepository.GetEmployeeByIdAsync(currentUserId) ?? throw new ApiException(OEMEVWarrantyManagement.Share.Models.Response.ResponseError.NotFoundEmployee);
            var role = _currentUserService.GetRole();

            var assigned = false;

            // If current user is SC staff and client provided assignment(s), validate and create work orders
            if (role == RoleIdEnum.ScStaff.GetRoleId() && (request.AssignTo != null || (request.AssignsTo != null && request.AssignsTo.Count > 0)))
            {
                // single assign
                if (request.AssignTo != null)
                {
                    var tech = await _employeeRepository.GetEmployeeByIdAsync(request.AssignTo.Value) ?? throw new ApiException(OEMEVWarrantyManagement.Share.Models.Response.ResponseError.NotFoundEmployee);
                    if (tech.OrgId != currentEmployee.OrgId) throw new ApiException(OEMEVWarrantyManagement.Share.Models.Response.ResponseError.Forbidden);

                    var workOrderEntity = new WorkOrder()
                    {
                        StartDate = DateTime.Now,
                        TargetId = warrantyClaim.ClaimId,
                        Type = WorkOrderType.Inspection.GetWorkOrderType(),
                        Target = WorkOrderTarget.Warranty.GetWorkOrderTarget(),
                        Status = WorkOrderStatus.InProgress.GetWorkOrderStatus(),
                        AssignedTo = request.AssignTo
                    };

                    _ = await _workOrderRepository.CreateAsync(workOrderEntity);
                    assigned = true;
                }

                // multiple assigns
                if (request.AssignsTo != null && request.AssignsTo.Count > 0)
                {
                    foreach (var techId in request.AssignsTo)
                    {
                        var tech = await _employeeRepository.GetEmployeeByIdAsync(techId) ?? throw new ApiException(OEMEVWarrantyManagement.Share.Models.Response.ResponseError.NotFoundEmployee);
                        if (tech.OrgId != currentEmployee.OrgId) throw new ApiException(OEMEVWarrantyManagement.Share.Models.Response.ResponseError.Forbidden);

                        var workOrderEntity = new WorkOrder()
                        {
                            StartDate = DateTime.Now,
                            TargetId = warrantyClaim.ClaimId,
                            Type = WorkOrderType.Inspection.GetWorkOrderType(),
                            Target = WorkOrderTarget.Warranty.GetWorkOrderTarget(),
                            Status = WorkOrderStatus.InProgress.GetWorkOrderStatus(),
                            AssignedTo = techId
                        };

                        _ = await _workOrderRepository.CreateAsync(workOrderEntity);
                    }

                    assigned = true;
                }

                if (assigned)
                {
                    warrantyClaim.Status = WarrantyClaimStatus.UnderInspection.GetWarrantyClaimStatus();
                    await _warrantyClaimRepository.UpdateAsync(warrantyClaim);
                }
                else
                {
                    // if no assigns provided, leave status as waiting for unassigned
                    warrantyClaim.Status = WarrantyClaimStatus.WaitingForUnassigned.GetWarrantyClaimStatus();
                    warrantyClaim.ConfirmBy = null;
                    warrantyClaim.ConfirmDate = null;
                    await _warrantyClaimRepository.UpdateAsync(warrantyClaim);
                }
            }
            else
            {
                // default behavior when not SC staff or no assign info: set back to waiting for unassigned
                warrantyClaim.Status = WarrantyClaimStatus.WaitingForUnassigned.GetWarrantyClaimStatus();
                warrantyClaim.ConfirmBy = null;
                warrantyClaim.ConfirmDate = null;
                await _warrantyClaimRepository.UpdateAsync(warrantyClaim);
            }

            var createdEntity = await _backWarrantyClaimRepository.CreateBackWarrantyClaimAsync(entity);
            return _mapper.Map<BackWarrantyClaimDto>(createdEntity);
        }

        public async Task<IEnumerable<BackWarrantyClaimDto>> GetAllBackWarrantyClaimsAsync()
        {
            var entities = await _backWarrantyClaimRepository.GetAllBackWarrantyClaimsAsync();
            //return entities.ContinueWith(t => _mapper.Map<IEnumerable<BackWarrantyClaimDto>>(t.Result));
            return _mapper.Map<IEnumerable<BackWarrantyClaimDto>>(entities);
        }
    }
}
