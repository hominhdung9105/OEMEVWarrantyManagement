﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OEMEVWarrantyManagement.Application.IServices
{
    public interface ICurrentUserService
    {
        Guid GetUserId();
    }
}
