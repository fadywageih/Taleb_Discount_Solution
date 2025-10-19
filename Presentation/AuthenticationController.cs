using Microsoft.AspNetCore.Mvc;
using ServicesAbstraction;
using Shared.Dtos;
using Shared.Dtos.User;

namespace Presentation
{
    [ApiController]
    [Route("/api/[controller]")]
    public class AuthenticationController(IServiceManager serviceManager) : ControllerBase
    {
        [HttpPost("register/school")]
        public async Task<ActionResult<UserResultDto>> RegisterSchool(SchoolRegisterDto schoolRegisterDto)
        {
            var result = await serviceManager.AuthenticationService.RegisterSchool(schoolRegisterDto);
            return Ok(result);
        }
        [HttpPost("register/university")]
        public async Task<ActionResult<UserResultDto>> RegisterUniversity(UniversityRegisterDto dto)
        {
            var result = await serviceManager.AuthenticationService.RegisterUniversity(dto);
            return Ok(result);
        }
        [HttpPost("register/vendor")]
        public async Task<ActionResult<UserResultDto>> RegisterVendor(VendorRegisterDto vendorRegisterDto)
        {
            var result = await serviceManager.AuthenticationService.RegisterVendor(vendorRegisterDto);
            return Ok(result);
        }
        [HttpGet("emailexists")]
        public async Task<ActionResult<bool>> CheckEmailExist(string email)
        {
            var result = await serviceManager.AuthenticationService.CheckIfEmailExist(email);
            return Ok(result);
        }
    }
}
