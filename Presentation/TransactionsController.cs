using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using ServicesAbstraction;
using Shared.Dtos.Transaction;
using System.Security.Claims;

namespace Presentation
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class TransactionsController : ControllerBase
    {
        private readonly IServiceManager _serviceManager;
        private readonly ILogger<TransactionsController> _logger;

        public TransactionsController(IServiceManager serviceManager, ILogger<TransactionsController> logger)
        {
            _serviceManager = serviceManager;
            _logger = logger;
        }

        [HttpPost]
        public async Task<ActionResult<TransactionDto>> CreateTransaction([FromBody] TransactionCreateDto dto)
        {
            try
            {
                var transaction = await _serviceManager.TransactionService.CreateTransactionAsync(dto);
                return CreatedAtAction(nameof(GetTransactionById), new { id = transaction.Id }, transaction);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating transaction");
                return BadRequest(new { error = ex.Message });
            }
        }

        [HttpGet("{id:guid}")]
        public async Task<ActionResult<TransactionDto>> GetTransactionById(Guid id)
        {
            try
            {
                var transaction = await _serviceManager.TransactionService.GetTransactionByIdAsync(id);
                return Ok(transaction);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting transaction by id");
                return NotFound(new { error = ex.Message });
            }
        }

        [HttpGet("vendor")]
        [Authorize(Roles = "Vendor")]
        public async Task<ActionResult<IEnumerable<TransactionDto>>> GetVendorTransactions()
        {
            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (string.IsNullOrEmpty(userId) || !Guid.TryParse(userId, out var userGuid))
                    return Unauthorized();

                var vendor = await _serviceManager.VendorService.GetVendorByUserIdAsync(userGuid);
                if (vendor == null)
                    return NotFound(new { message = "Vendor not found" });

                var transactions = await _serviceManager.TransactionService.GetVendorTransactionsAsync(vendor.Id);
                return Ok(transactions);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting vendor transactions");
                return StatusCode(500, new { error = "An error occurred" });
            }
        }

        [HttpGet("customer")]
        [Authorize(Roles = "User,School,University")]
        public async Task<ActionResult<IEnumerable<TransactionDto>>> GetCustomerTransactions()
        {
            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (string.IsNullOrEmpty(userId) || !Guid.TryParse(userId, out var userGuid))
                    return Unauthorized();

                var transactions = await _serviceManager.TransactionService.GetCustomerTransactionsAsync(userGuid);
                return Ok(transactions);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting customer transactions");
                return StatusCode(500, new { error = "An error occurred" });
            }
        }

        [HttpPost("cancel")]
        [Authorize(Roles = "User,School,University")]
        public async Task<ActionResult<TransactionDto>> CancelTransaction([FromBody] TransactionUpdateStatusDto dto)
        {
            try
            {
                var transaction = await _serviceManager.TransactionService.CancelTransactionAsync(dto.TransactionId);
                return Ok(transaction);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error canceling transaction");
                return BadRequest(new { error = ex.Message });
            }
        }

        [HttpPost("accept")]
        [Authorize(Roles = "Vendor")]
        public async Task<ActionResult<TransactionDto>> AcceptTransaction([FromBody] TransactionUpdateStatusDto dto)
        {
            try
            {
                var transaction = await _serviceManager.TransactionService.AcceptTransactionAsync(dto.TransactionId);
                return Ok(transaction);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error accepting transaction");
                return BadRequest(new { error = ex.Message });
            }
        }

        [HttpPost("reject")]
        [Authorize(Roles = "Vendor")]
        public async Task<ActionResult<TransactionDto>> RejectTransaction([FromBody] TransactionUpdateStatusDto dto)
        {
            try
            {
                var transaction = await _serviceManager.TransactionService.RejectTransactionAsync(
                    dto.TransactionId,
                    dto.RejectionReason ?? "No reason provided");
                return Ok(transaction);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error rejecting transaction");
                return BadRequest(new { error = ex.Message });
            }
        }

        [HttpGet("vendor/stats")]
        [Authorize(Roles = "Vendor")]
        public async Task<ActionResult<TransactionStatsDto>> GetVendorStats()
        {
            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (string.IsNullOrEmpty(userId) || !Guid.TryParse(userId, out var userGuid))
                    return Unauthorized();

                var vendor = await _serviceManager.VendorService.GetVendorByUserIdAsync(userGuid);
                if (vendor == null)
                    return NotFound(new { message = "Vendor not found" });

                var stats = await _serviceManager.TransactionService.GetVendorStatsAsync(vendor.Id);
                return Ok(stats);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting vendor stats");
                return StatusCode(500, new { error = "An error occurred" });
            }
        }
    }
}