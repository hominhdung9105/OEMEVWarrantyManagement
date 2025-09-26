using OEMEVWarrantyManagement.Application.Dtos;

namespace OEMEVWarrantyManagement.Application.IServices
{
    public interface ICarConditionService
    {
        Task<IEnumerable<CarConditionCurrentDto>> GetAllAsync();
        Task<IEnumerable<CarConditionCurrentDto>> GetAllByStaffAsync(string staffId);
        Task<CarConditionCurrentDto?> GetAsync(string warrantyRequestId);
        Task<CarConditionCurrentDto?> CreateAsync(Guid warrantyRequestId);
        Task<CarConditionCurrentDto?> UpdateAsync(Guid employeeId, string warrantyRequestId, CarConditionCurrentDto request);

    }
}
