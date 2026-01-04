using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using ServicesAbstraction;
using Shared;
using Shared.Dtos.Product;
using System.Security.Claims;

namespace Presentation.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductsController : ControllerBase
    {
        private readonly IServiceManager _serviceManager;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILogger<ProductsController> _logger;

        public ProductsController(IServiceManager serviceManager,
            IHttpContextAccessor httpContextAccessor,
            ILogger<ProductsController> logger)
        {
            _serviceManager = serviceManager;
            _httpContextAccessor = httpContextAccessor;
            _logger = logger;
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> GetAllProducts(
            [FromQuery] ProductParameterSpecifications parameters)
        {
            try
            {
                _logger.LogInformation("Getting all products with parameters: {@Parameters}", parameters);

                var result = await _serviceManager.ProductService.GetAllProductsAsync(parameters);

                _logger.LogInformation("Successfully retrieved {Count} products", result.Data?.Count() ?? 0);

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting all products");
                return StatusCode(500, new { error = "An error occurred while getting products" });
            }
        }

        [HttpGet("{id}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetProduct(int id)
        {
            try
            {
                var product = await _serviceManager.ProductService.GetProductByIdAsync(id);
                return Ok(product);
            }
            
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }

        [HttpPost]
        [Authorize(Roles = "Vendor")]
        public async Task<IActionResult> CreateProduct([FromForm] ProductCreateDto productDto)
        {
            try
            {
                var vendorId = GetCurrentVendorId();
                var result = await _serviceManager.ProductService.CreateProductAsync(productDto, vendorId);
                return CreatedAtAction(nameof(GetProduct), new { id = result.Id }, result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Vendor")]
        public async Task<IActionResult> UpdateProduct(int id, [FromForm] ProductUpdateDto productDto)
        {
            try
            {
                if (id != productDto.Id)
                    return BadRequest("ID mismatch");

                await _serviceManager.ProductService.UpdateProductAsync(productDto);
                return NoContent();
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Vendor")]
        public async Task<IActionResult> DeleteProduct(int id)
        {
            try
            {
                await _serviceManager.ProductService.DeleteProductAsync(id);
                return NoContent();
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        [HttpGet("vendor")]
        [Authorize(Roles = "Vendor")]
        public async Task<IActionResult> GetVendorProducts()
        {
            try
            {
                var vendorId = GetCurrentVendorId();
                var products = await _serviceManager.ProductService.GetVendorProductsAsync(vendorId);
                return Ok(products);
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        [HttpGet("best-selling")]
        [AllowAnonymous]
        public async Task<IActionResult> GetBestSellingProducts()
        {
            try
            {
                // يمكنك تعديل المعلمات للحصول على المنتجات الأكثر مبيعاً
                var parameters = new ProductParameterSpecifications
                {
                    PageIndex = 1,
                    PageSize = 10,
                    Sort = ProductSortOption.DiscountDesc // أو استخدام معيار آخر
                };

                var result = await _serviceManager.ProductService.GetAllProductsAsync(parameters);
                return Ok(result.Data?.Take(8)); // أخذ أفضل 8 منتجات
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }

        [HttpGet("category/{categoryId}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetProductsByCategory(int categoryId)
        {
            try
            {
                var parameters = new ProductParameterSpecifications
                {
                    CategoryId = categoryId,
                    PageIndex = 1,
                    PageSize = 20
                };

                var result = await _serviceManager.ProductService.GetAllProductsAsync(parameters);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }

        private Guid GetCurrentVendorId()
        {
            var userId = _httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userId) || !Guid.TryParse(userId, out var userGuid))
            {
                throw new UnauthorizedAccessException("Invalid user ID");
            }

            var vendor = _serviceManager.VendorService.GetVendorByUserIdAsync(userGuid).Result;

            if (vendor == null)
            {
                throw new UnauthorizedAccessException("Vendor profile not found for this user");
            }

            return vendor.Id;
        }
    }
}