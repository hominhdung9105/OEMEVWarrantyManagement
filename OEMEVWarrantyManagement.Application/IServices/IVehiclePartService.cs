using OEMEVWarrantyManagement.Application.Dtos;
using OEMEVWarrantyManagement.Domain.Entities;

namespace OEMEVWarrantyManagement.Application.IServices
{
    public interface IVehiclePartService
    {
        Task<IEnumerable<string>> GetSerialsByVinAndPartModelAsync(string vin, string partModel);
    }
}
