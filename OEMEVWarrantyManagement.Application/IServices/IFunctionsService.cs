using OEMEVWarrantyManagement.Share.Enum;

namespace OEMEVWarrantyManagement.Application.IServices
{
    public interface IFunctionsService
    {
        List<RoleScreenPermission> GetFunctions(string roleId);
    }
}
