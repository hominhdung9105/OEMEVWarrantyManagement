namespace OEMEVWarrantyManagement.Application.IServices
{
    public interface IVehiclePartService
    {
        Task<IEnumerable<string>> GetSerialsByVinAndPartModelAsync(string vin, string partModel);
    }
}
