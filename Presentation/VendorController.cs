using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ServicesAbstraction;
using Shared.Dtos.Vendor;
using System.Security.Claims;

namespace Presentation.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class VendorController : ControllerBase
    {
        private readonly IServiceManager _serviceManager;

        public VendorController(IServiceManager serviceManager)
        {
            _serviceManager = serviceManager;
        }

        [HttpGet("profile")]
        public async Task<ActionResult<VendorDto>> GetProfile()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(userId) || !Guid.TryParse(userId, out var userGuid))
                return Unauthorized(new { message = "Invalid user ID" });

            var vendor = await _serviceManager.VendorService.GetVendorByUserIdAsync(userGuid);

            return vendor == null ? NotFound(new { message = "Vendor profile not found" }) : Ok(vendor);
        }

        [HttpGet("{id:guid}")]
        public async Task<ActionResult<VendorDto>> GetById(Guid id)
        {
            var vendor = await _serviceManager.VendorService.GetVendorByIdAsync(id);
            return vendor == null ? NotFound() : Ok(vendor);
        }

        [HttpPut("profile")]
        public async Task<ActionResult<VendorDto>> UpdateProfile([FromBody] UpdateVendorDto vendorDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId) || !Guid.TryParse(userId, out var userGuid))
                return Unauthorized();

            var existingVendor = await _serviceManager.VendorService.GetVendorByUserIdAsync(userGuid);
            if (existingVendor == null)
                return NotFound();

            var vendor = await _serviceManager.VendorService.UpdateVendorAsync(existingVendor.Id, vendorDto);
            return vendor == null ? NotFound() : Ok(vendor);
        }
    }
}