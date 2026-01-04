using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ServicesAbstraction;
using Shared.Dtos.FeedBack;
namespace Presentation.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class FeedBackController : ControllerBase
    {
        private readonly IServiceManager _serviceManager;

        public FeedBackController(IServiceManager serviceManager)
        {
            _serviceManager = serviceManager;
        }
        [HttpPost]
        [AllowAnonymous]
        public async Task<ActionResult<FeedBackDto>> CreateFeedBack(FeedBackCreateDto dto)
        {
            try
            {
                var result = await _serviceManager.FeedBackService.CreateFeedBackAsync(dto);
                return Ok(result);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { error = ex.Message });
            }
            catch (Exception)
            {
                return StatusCode(500, new { error = "An error occurred while saving your feedback" });
            }
        }
        [HttpGet("{id}")]
        [AllowAnonymous]
        public async Task<ActionResult<FeedBackDto>> GetFeedBackById(Guid id)
        {
            var feedBack = await _serviceManager.FeedBackService.GetFeedBackByIdAsync(id);
            if (feedBack == null)
                return NotFound();

            return Ok(feedBack);
        }
        [HttpGet("email/{email}")]
        [AllowAnonymous]
        public async Task<ActionResult<IEnumerable<FeedBackDto>>> GetFeedBacksByEmail(string email)
        {
            var feedBacks = await _serviceManager.FeedBackService.GetFeedBacksByEmailAsync(email);
            return Ok(feedBacks);
        }
        [HttpGet("statistics")]
        [Authorize(Roles = "Admin,Vendor")]
        public async Task<ActionResult<FeedBackStatsDto>> GetStatistics()
        {
            var stats = await _serviceManager.FeedBackService.GetFeedBackStatisticsAsync();
            return Ok(stats);
        }
        [HttpGet("recent")]
        [Authorize(Roles = "Admin,Vendor")]
        public async Task<ActionResult<IEnumerable<FeedBackDto>>> GetRecentFeedBacks([FromQuery] int count = 10)
        {
            var feedBacks = await _serviceManager.FeedBackService.GetRecentFeedBacksAsync(count);
            return Ok(feedBacks);
        }
    }
}