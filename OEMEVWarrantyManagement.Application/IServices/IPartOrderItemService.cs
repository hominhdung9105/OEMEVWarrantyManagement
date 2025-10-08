using OEMEVWarrantyManagement.Application.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OEMEVWarrantyManagement.Application.IServices
{
    public interface IPartOrderItemService
    {
        Task<PartOrderItemDto> CreateAsync(RequsetPartOrderItemDto partOrderItemDto);
    }
}
