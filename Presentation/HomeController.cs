using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ServicesAbstraction;
using Shared.Dtos.User.Shared.Dtos.Home;
using System.Security.Claims;

namespace Presentation.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class HomeController : ControllerBase
    {
        private readonly IServiceManager _serviceManager;
        public HomeController(IServiceManager serviceManager)
        {
            _serviceManager = serviceManager;
        }
        [HttpGet]
        [AllowAnonymous]
        public async Task<ActionResult<HomePageDto>> GetHomePageData()
        {
            var homeData = await _serviceManager.HomeService.GetHomePageDataAsync();
            return Ok(homeData);
        }
        [HttpGet("student")]
        [Authorize(Roles = "School,University")]
        public async Task<ActionResult<HomePageDto>> GetStudentHomePageData()
        {
            var userType = User.FindFirstValue("UserType");
            if (string.IsNullOrEmpty(userType))
                return Unauthorized(new { message = "User type not found" });

            var homeData = await _serviceManager.HomeService.GetHomePageDataForStudentAsync(userType);
            return Ok(homeData);
        }
    }
}