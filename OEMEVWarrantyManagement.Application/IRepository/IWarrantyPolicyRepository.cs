﻿using OEMEVWarrantyManagement.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OEMEVWarrantyManagement.Application.IRepository
{
    public interface IWarrantyPolicyRepository
    {
        Task<IEnumerable<WarrantyPolicy>> GetAllAsync();
        Task<WarrantyPolicy> GetByIdAsync(Guid policyId);
        IQueryable<WarrantyPolicy> Query();
    }
}
