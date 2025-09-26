using AutoMapper;
using OEMEVWarrantyManagement.Application.Dtos;
using OEMEVWarrantyManagement.Application.IRepository;
using OEMEVWarrantyManagement.Application.IServices;
using OEMEVWarrantyManagement.Share.Enum;
using OEMEVWarrantyManagement.Share.Exceptions;
using OEMEVWarrantyManagement.Share.Models.Response;

namespace OEMEVWarrantyManagement.Application.Services
{
    public class CarConditionService(ICarConditionRepository carConditionRepository, IEmployeeRepository employeeRepository, IMapper mapper, IWarrantyRequestService warrantyRequestService) : ICarConditionService
    {
        public async Task<CarConditionCurrentDto?> CreateAsync(Guid warrantyRequestId)
        {
            if (await carConditionRepository.GetByIdAsync(warrantyRequestId) != null)
            {
                throw new ApiException(ResponseError.DuplicateCarCondition);
            }

            if(await warrantyRequestService.GetByIdAsync(warrantyRequestId) is null)
                throw new ApiException(ResponseError.NotfoundWarrantyRequest);

            var carCondition = await carConditionRepository.CreateAsync(warrantyRequestId.ToString());

            await warrantyRequestService.UpdateAsync(new WarrantyRequestDto { Id = warrantyRequestId, Status = WarrantyRequestStatus.WaitingForUnassigned.GetWarrantyRequestStatus() });

            var res = mapper.Map<CarConditionCurrentDto>(carCondition);
            return res;
        }

        public async Task<IEnumerable<CarConditionCurrentDto>> GetAllAsync()
        {
            var listCarCondition = await carConditionRepository.GetAllAsync();
            var res = mapper.Map<IEnumerable<CarConditionCurrentDto>>(listCarCondition);
            return res;
        }

        public async Task<IEnumerable<CarConditionCurrentDto>> GetAllByStaffAsync(string staffId)
        {
            var listCarCondition = await carConditionRepository.GetAllAsync(staffId);
            var res = mapper.Map<IEnumerable<CarConditionCurrentDto>>(listCarCondition);
            return res;
        }

        public async Task<CarConditionCurrentDto?> GetAsync(string warrantyRequestId)
        {
            var carCondition = await carConditionRepository.GetByIdAsync(Guid.Parse(warrantyRequestId)) ?? throw new ApiException(ResponseError.NotFoundCarCondition);
            var res = mapper.Map<CarConditionCurrentDto>(carCondition);
            return res;
        }

        public async Task<CarConditionCurrentDto?> UpdateAsync(Guid employeeId, string warrantyRequestId, CarConditionCurrentDto request)
        {
            var carCondition = await carConditionRepository.GetByIdAsync(Guid.Parse(warrantyRequestId)) ?? throw new ApiException(ResponseError.NotFoundCarCondition);

            var employee = await employeeRepository.GetByIdAsync(employeeId) ?? throw new ApiException(ResponseError.NotFoundEmployee);

            if (request.TechnicianId != null && employee.RoleId == RoleIdEnum.ScStaff.GetRoleId())
            {
                try
                {
                    var techId = Guid.Parse(request.TechnicianId.ToString());
                
                
                    var tech = await employeeRepository.GetByIdAsync(techId) ?? throw new ApiException(ResponseError.NotFoundEmployee);

                    if (tech.RoleId != "ROL-TECH")
                        throw new ApiException(ResponseError.EmployeeNotTech);

                    await warrantyRequestService.UpdateAsync(new WarrantyRequestDto { Id = Guid.Parse(warrantyRequestId), Status = WarrantyRequestStatus.UnderInspection.GetWarrantyRequestStatus() });

                    carCondition.TechnicianId = (Guid)request.TechnicianId;
                }
                catch (Exception)
                {
                    throw new ApiException(ResponseError.NotFoundEmployee);
                }
            }
            else if (carCondition.TechnicianId != null && carCondition.TechnicianId == employeeId)
            {
                if (request.Condition != null)
                {
                    carCondition.Condition = request.Condition;
                }

                if (request.Detail != null)
                {
                    carCondition.Detail = request.Detail;
                }

                await warrantyRequestService.UpdateAsync(new WarrantyRequestDto { Id = Guid.Parse(warrantyRequestId), Status = WarrantyRequestStatus.PendingConfirmation.GetWarrantyRequestStatus() });
            }
            else
            {
                throw new ApiException(ResponseError.InvalidUpdateCarCondition);
            }

            await carConditionRepository.SaveChangeAsync();

            var res = mapper.Map<CarConditionCurrentDto>(carCondition);
            return res;
        }
    }
}
