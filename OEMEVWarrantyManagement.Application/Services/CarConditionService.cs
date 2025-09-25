using OEMEVWarrantyManagement.Application.Dtos;
using OEMEVWarrantyManagement.Application.IRepository;
using OEMEVWarrantyManagement.Application.IServices;
using OEMEVWarrantyManagement.Share.Exceptions;
using OEMEVWarrantyManagement.Share.Models.Response;

namespace OEMEVWarrantyManagement.Application.Services
{
    public class CarConditionService(ICarConditionRepository carConditionRepository, IEmployeeRepository employeeRepository) : ICarConditionService
    {
        //public async Task<CarConditionCurrentDto?> CreateAsync(string staffId, string warrantyRequestId)
        //{
        //    if (await carConditionRepository.GetAsync(warrantyRequestId) != null)
        //    {
        //        throw new ApiException(ResponseError.DuplicateCarCondition);
        //    }

        //    var carCondition = await carConditionRepository.CreateAsync(warrantyRequestId);

        //    return carCondition;
        //}

        public async Task<IEnumerable<CarConditionCurrentDto>> GetAllAsync()
        {
            var listCarCondition = await carConditionRepository.GetAllAsync();
            // TODO - mapping
            return listCarCondition;
        }

        public async Task<IEnumerable<CarConditionCurrentDto>> GetAllByStaffAsync(string staffId)
        {
            var listCarCondition = await carConditionRepository.GetAllAsync(staffId);
            // TODO - mapping
            return listCarCondition;
        }

        public async Task<CarConditionCurrentDto?> GetAsync(string warrantyRequestId)
        {
            var carCondition = await carConditionRepository.GetByIdAsync(warrantyRequestId) ?? throw new ApiException(ResponseError.NotFoundCarCondition);
            // TODO - mapping
            return carCondition;
        }

        public async Task<CarConditionCurrentDto?> UpdateAsync(string staffId, string warrantyRequestId, CarConditionCurrentDto request)
        {
            var carCondition = await carConditionRepository.GetByIdAsync(warrantyRequestId) ?? throw new ApiException(ResponseError.NotFoundCarCondition);

            if (request.TechnicianId != null)
            {
                var employee = await employeeRepository.GetByIdAsync(request.TechnicianId.ToString()) ?? throw new ApiException(ResponseError.NotfoundEmployee);
                carCondition.TechnicianId = request.TechnicianId;
            }
            

            if (request.Condition != null)
            {
                carCondition.Condition = request.Condition;
            }

            if(request.Detail != null)
            {
                carCondition.Detail = request.Detail;
            }

            

            await carConditionRepository.SaveChangeAsync();
            // TODO - mapping
            return carCondition;
        }
    }
}
