using OEMEVWarrantyManagement.API.Models;
using OEMEVWarrantyManagement.Database.Models;
using System.Collections;

namespace OEMEVWarrantyManagement.API.Services
{
    public interface IWarrantyRecordService
    {
        Task <IEnumerable<WarrantyRecordDto>> GetAllAsync();
        Task <IEnumerable<WarrantyRecordDto>> GetByVINAsync (string VIN);
    }
}
