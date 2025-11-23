//using OEMEVWarrantyManagement.Application.IServices;
//using OEMEVWarrantyManagement.Application.IRepository;
//using OEMEVWarrantyManagement.Share.Enums;
//using OEMEVWarrantyManagement.Share.Exceptions;
//using OEMEVWarrantyManagement.Share.Models.Response;

//namespace OEMEVWarrantyManagement.Application.Services
//{
//    public class VehiclePartService : IVehiclePartService
//    {
//        private readonly IVehicleRepository _vehicleRepository;
//        //private readonly IVehiclePartRepository _vehiclePartRepository;

//        public VehiclePartService(
//            IVehicleRepository vehicleRepository)
//            //IVehiclePartRepository vehiclePartRepository)
//        {
//            _vehicleRepository = vehicleRepository;
//            //_vehiclePartRepository = vehiclePartRepository;
//        }

//        public async Task<IEnumerable<string>> GetSerialsByVinAndPartModelAsync(string vin, string partModel)
//        {
//            var vehicle = await _vehicleRepository.GetVehicleByVinAsync(vin) ?? throw new ApiException(ResponseError.NotfoundVin);

//            if (!PartModel.IsValidModel(partModel))
//                throw new ApiException(ResponseError.InvalidPartModel);

//            // Repository already filters by VehiclePartCurrentStatus.OnVehicle
//            var parts = await _vehiclePartRepository.GetVehiclePartByVinAndModelAsync(vin, partModel);
//            return parts.Select(p => p.SerialNumber);
//        }
//    }
//}
