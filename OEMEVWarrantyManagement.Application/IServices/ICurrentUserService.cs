namespace OEMEVWarrantyManagement.Application.IServices
{
    public interface ICurrentUserService
    {
        Guid GetUserId();
        String GetRole();
        Task<Guid> GetOrgId();
    }
}
