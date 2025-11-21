using Microsoft.AspNetCore.Mvc;
using OEMEVWarrantyManagement.Application.IServices;

namespace OEMEVWarrantyManagement.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ClaimPartController : ControllerBase
    {
        private readonly IClaimPartService _claimPartService;
        public ClaimPartController(IClaimPartService claimPartService)
        {
            _claimPartService = claimPartService;
        }

    }
}
