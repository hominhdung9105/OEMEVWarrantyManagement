using OEMEVWarrantyManagement.Domain.Entities;

namespace OEMEVWarrantyManagement.Application.IRepository
{
    public interface ICustomerRepository
    {
        Task<Customer> GetCustomerByIdAsync(Guid customerId);
        Task<List<Customer>> GetCustomersByIdsAsync(List<Guid> customerIds);

    }
}
