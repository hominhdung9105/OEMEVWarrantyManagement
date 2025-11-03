using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OEMEVWarrantyManagement.Application.Dtos;
using OEMEVWarrantyManagement.Application.IServices;
using OEMEVWarrantyManagement.Share.Enums;
using OEMEVWarrantyManagement.Share.Exceptions;
using OEMEVWarrantyManagement.Share.Models.Pagination;
using OEMEVWarrantyManagement.Share.Models.Response;

namespace OEMEVWarrantyManagement.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class WarrantyClaimController : ControllerBase
    {
        private readonly IWarrantyClaimService _warrantyClaimService;
        private readonly IEmployeeService _employeeService;
        private readonly IWorkOrderService _workOrderService;
        private readonly IClaimPartService _claimPartService;
        private readonly IVehicleWarrantyPolicyService _vehicleWarrantyPolicyService;
        public WarrantyClaimController(IWarrantyClaimService warrantyClaimService, IEmployeeService employeeService, IWorkOrderService workOrderService, IClaimPartService claimPartService, IVehicleWarrantyPolicyService vehicleWarrantyPolicyService)
        {
            _warrantyClaimService = warrantyClaimService;
            _employeeService = employeeService;
            _workOrderService = workOrderService;
            _claimPartService = claimPartService;
            _vehicleWarrantyPolicyService = vehicleWarrantyPolicyService;
        }

        //create : VIN
        [HttpPost]
        [Authorize(policy: "RequireScStaff")]
        public async Task<IActionResult> Create([FromBody] RequestWarrantyClaim request)
        {
            var result = await _warrantyClaimService.CreateAsync(request);
            return Ok(ApiResponse<object>.Ok(result, "Create Warranty Claim Successfully!"));
        }

        // Unified GET with filters: search (VIN, customer), status. Role-aware: evm, admin -> all; sc staff -> only their org; tech -> forbidden
        [HttpGet]
        [Authorize]
        public async Task<IActionResult> Get([FromQuery] PaginationRequest request, [FromQuery] string? search, [FromQuery] string? status)
        {
            var result = await _warrantyClaimService.GetPagedUnifiedAsync(request, search, status);
            return Ok(ApiResponse<PagedResult<ResponseWarrantyClaimDto>>.Ok(result, "Get Warranty Claims Successfully!"));
        }

        [HttpGet("{vin}/vehicle-policies")]
        [Authorize]
        public async Task<IActionResult> GetVehiclePolicies(string vin)
        {
            var policies = await _vehicleWarrantyPolicyService.GetAllByVinAsync(vin);
            return Ok(ApiResponse<IEnumerable<VehicleWarrantyPolicyDto>>.Ok(policies, "Get vehicle policies"));
        }

        [HttpPut("{claimId}/approve")]
        [Authorize(policy: "RequireEvmStaff")]
        public async Task<IActionResult> ApproveWarrantyClaim(string claimId, [FromBody] ApproveWarrantyClaimRequest request)
        {
            if (!Guid.TryParse(claimId, out var id)) throw new ApiException(ResponseError.InvalidWarrantyClaimId);
            if (request.VehicleWarrantyId == null) throw new ApiException(ResponseError.InvalidVehiclePolicyId);
            if (!Guid.TryParse(request?.VehicleWarrantyId, out var vehicleWarrantyId)) throw new ApiException(ResponseError.InvalidVehiclePolicyId);

            var result = await _warrantyClaimService.UpdateStatusAsync(id, WarrantyClaimStatus.Approved, vehicleWarrantyId);

            return Ok(ApiResponse<WarrantyClaimDto>.Ok(result, "Accept Successfully!"));
        }

        [HttpPut("{claimId}/deny")]
        [Authorize(policy: "RequireScStaffOrEvmStaff")]
        public async Task<IActionResult> DenyWarrantyClaim(string claimId)
        {
            if (!Guid.TryParse(claimId, out var id)) throw new ApiException(ResponseError.InvalidWarrantyClaimId);

            var result = await _warrantyClaimService.UpdateStatusAsync(id, WarrantyClaimStatus.Denied);

            return Ok(ApiResponse<WarrantyClaimDto>.Ok(result, "Deny Successfully!"));
        }

        [HttpPut("{claimId}/send-to-manufacturer")]
        [Authorize(policy: "RequireScStaff")]
        public async Task<IActionResult> SendToManufacturerWarrantyClaim(string claimId)
        {
            if (!Guid.TryParse(claimId, out var id)) throw new ApiException(ResponseError.InvalidWarrantyClaimId);

            var result = await _warrantyClaimService.UpdateStatusAsync(id, WarrantyClaimStatus.SentToManufacturer);

            return Ok(ApiResponse<WarrantyClaimDto>.Ok(result, "Send to Manufacturer Successfully!"));
        }

        [HttpPut("{claimId}/done-warranty")]
        [Authorize(policy: "RequireScStaff")]
        public async Task<IActionResult> StartInspectionWarrantyClaim(string claimId)
        {
            if (!Guid.TryParse(claimId, out var id)) throw new ApiException(ResponseError.InvalidWarrantyClaimId);

            var result = await _warrantyClaimService.UpdateStatusAsync(id, WarrantyClaimStatus.DoneWarranty);

            return Ok(ApiResponse<WarrantyClaimDto>.Ok(result, "Start Inspection Successfully!"));
        }

        [HttpPut("{claimId}/inspection")]
        [Authorize(policy: "RequireScTech")]
        public async Task<IActionResult> UpdateDescriptionWarrantyClaim(string claimId, [FromBody] InspectionDto request)
        {
            if (!Guid.TryParse(claimId, out var id)) throw new ApiException(ResponseError.InvalidWarrantyClaimId);

            // Validate description
            if (string.IsNullOrWhiteSpace(request.Description))
            {
                throw new ApiException(ResponseError.InvalidDescription);
            }

            var result = await _warrantyClaimService.UpdateDescription(id, request.Description);
            await _claimPartService.CreateManyClaimPartsAsync(id, request.Parts);

            return Ok(ApiResponse<WarrantyClaimDto>.Ok(result, "Update Description Successfully!"));
        }

        [HttpPut("{claimId}/car-back-home")]
        [Authorize(policy: "RequireScStaff")]
        public async Task<IActionResult> CarBackHomeWarrantyClaim(string claimId)
        {
            if (!Guid.TryParse(claimId, out var id)) throw new ApiException(ResponseError.InvalidWarrantyClaimId);

            var result = await _warrantyClaimService.UpdateStatusAsync(id, WarrantyClaimStatus.CarBackHome);

            return Ok(ApiResponse<WarrantyClaimDto>.Ok(result, "Update Successfully!"));
        }

        [HttpPut("{claimId}/car-back-center")]
        [Authorize(policy: "RequireScStaff")]
        public async Task<IActionResult> CarBackCenterWarrantyClaim(string claimId)
        {
            if (!Guid.TryParse(claimId, out var id)) throw new ApiException(ResponseError.InvalidWarrantyClaimId);

            var result = await _warrantyClaimService.UpdateStatusAsync(id, WarrantyClaimStatus.Approved);

            // TODO - Approved rồi check kho có đủ phụ tùng không

            return Ok(ApiResponse<WarrantyClaimDto>.Ok(result, "Update Successfully!"));
        }

        [HttpPut("{claimId}/repair")]
        [Authorize(policy: "RequireScTech")]
        public async Task<IActionResult> RepairWarrantyClaim(string claimId, [FromBody] RepairRequestDto request)
        {
            if (!Guid.TryParse(claimId, out var id)) throw new ApiException(ResponseError.InvalidWarrantyClaimId);

            request.ClaimId = id;

            var result = await _warrantyClaimService.UpdateStatusAsync(id, WarrantyClaimStatus.Repaired);

            await _claimPartService.UpdateClaimPartsAsync(request);

            return Ok(ApiResponse<WarrantyClaimDto>.Ok(result, "Update Successfully!"));
        }

        [HttpGet("status")]
        [Authorize]
        public IActionResult GetAllWarrantyClaimStatuses()
        {
            var statuses = WarrantyClaimStatusExtensions.GetAllStatus();
            return Ok(ApiResponse<IEnumerable<string>>.Ok(statuses, "Get All Warranty Claim Statuses Successfully!"));
        }

        // New: assign technicians to a warranty claim (inspection or repair based on claim status)
        [HttpPost("{claimId}/assign-techs")]
        [Authorize(policy: "RequireScStaff")]
        public async Task<IActionResult> AssignTechniciansToClaim(string claimId, [FromBody] AssignTechRequestDto request)
        {
            if (!Guid.TryParse(claimId, out var id)) throw new ApiException(ResponseError.InvalidWarrantyClaimId);
            if (request?.AssignedTo == null || request.AssignedTo.Count == 0) throw new ApiException(ResponseError.InvalidJsonFormat);

            var result = await _workOrderService.CreateForWarrantyAsync(id, request.AssignedTo);
            return Ok(ApiResponse<IEnumerable<WorkOrderDto>>.Ok(result, "Assign technicians successfully!"));
        }
    }
}
