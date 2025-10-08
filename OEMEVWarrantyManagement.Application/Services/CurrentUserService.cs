using Microsoft.AspNetCore.Http;
using OEMEVWarrantyManagement.Application.IServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace OEMEVWarrantyManagement.Application.Services
{
    public class CurrentUserService : ICurrentUserService
    {
        private readonly IHttpContextAccessor _context;

        public CurrentUserService(IHttpContextAccessor context)
        {
            _context = context;
        }

        public Guid GetUserId() =>
            Guid.Parse(_context.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier));
    }
}
