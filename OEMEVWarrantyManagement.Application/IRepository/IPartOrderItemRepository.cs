using OEMEVWarrantyManagement.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OEMEVWarrantyManagement.Application.IRepository
{
    public interface IPartOrderItemRepository
    {
        Task<PartOrderItem> CreateAsync(PartOrderItem request);
        Task<IEnumerable<PartOrderItem>> GetAllByOrderIdAsync(Guid orderId);
    }
}
