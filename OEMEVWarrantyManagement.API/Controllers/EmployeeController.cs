using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OEMEVWarrantyManagement.Application.Dtos;
using OEMEVWarrantyManagement.Application.IServices;
using OEMEVWarrantyManagement.Share.Exceptions;
using OEMEVWarrantyManagement.Share.Models.Response;
using System.Security.Claims;

namespace OEMEVWarrantyManagement.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmployeeController : ControllerBase
    {
        private readonly IEmployeeService _employeeService;
        public EmployeeController(IEmployeeService employeeService)
        {
            _employeeService = employeeService;
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetAllTech()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var orgEmployee = await _employeeService.GetEmployeeByIdAsync(Guid.Parse(userId));
            var result = await _employeeService.GetAllTechInWorkspaceAsync(orgEmployee.OrgId);
            return Ok(ApiResponse<object>.Ok(result, "Get all tech in the same Workspace"));
        }

        [HttpGet("accounts")]
        [Authorize(Policy = "RequireAdmin")]
        public async Task<IActionResult> GetAllAccounts()
        {
            var accounts = await _employeeService.GetAllAccountsAsync();
            return Ok(ApiResponse<IEnumerable<EmployeeDto>>.Ok(accounts, "Get all accounts successfully"));
        }

        [HttpGet("policies")]
        [Authorize(Policy = "RequireAdmin")]
        public async Task<IActionResult> GetAllPolicies([FromServices] IWarrantyPolicyService policyService)
        {
            var policies = await policyService.GetAllAsync();
            return Ok(ApiResponse<IEnumerable<WarrantyPolicyDto>>.Ok(policies, "Get all policies successfully"));
        }

        [HttpPost("createAccount")]
        [Authorize(policy: "RequireAdmin")]
        public async Task<ActionResult> Create(EmployeeDto request)
        {
            var employee = await _employeeService.CreateAccountAsync(request) ?? throw new ApiException(ResponseError.UsernameAlreadyExists);
            return Ok(ApiResponse<object>.Ok(employee, "Create account suscessfull!"));
        }

        [HttpPut("updateAccount/{id}")]
        [Authorize(policy: "RequireAdmin")]
        public async Task<ActionResult> Update(string id, EmployeeDto request)
        {
            var employee = await _employeeService.UpdateAccountAsync(id, request) ?? throw new ApiException(ResponseError.EmployeeNotFound);
            return Ok(ApiResponse<object>.Ok(employee, "Update account suscessfull!"));
        }

        [HttpDelete("deleteAccount/{id:guid}")]
        [Authorize(Policy = "RequireAdmin")]
        public async Task<IActionResult> DeleteAccount(Guid id)
        {
            var deleted = await _employeeService.DeleteAccountAsync(id);
            if (!deleted)
                return NotFound(ApiResponse<object>.Fail(ResponseError.EmployeeNotFound));
            return Ok(ApiResponse<object>.Ok(null, "Delete account successfully"));
        }
    }
}
