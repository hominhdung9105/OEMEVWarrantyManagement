using AutoMapper;
using OEMEVWarrantyManagement.Application.Dtos;
using OEMEVWarrantyManagement.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace OEMEVWarrantyManagement.Application.Mapping
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            //Warranty Request
            CreateMap<WarrantyRequest, WarrantyRequestDto>();
            CreateMap<WarrantyRequestDto, WarrantyRequest>();
        }
    }
}
