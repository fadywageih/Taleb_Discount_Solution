    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using ServicesAbstraction;
    using Shared.Dtos.Product;
    using System.Security.Claims;

    [ApiController]
    [Route("api/[controller]")]
    public class ProductsController : ControllerBase
    {
        private readonly IServiceManager _serviceManager;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public ProductsController(IServiceManager serviceManager, IHttpContextAccessor httpContextAccessor)
        {
            _serviceManager = serviceManager;
            _httpContextAccessor = httpContextAccessor;
        }

        [HttpPost]
        [Authorize(Roles = "Vendor")]
        public async Task<IActionResult> CreateProduct([FromForm] ProductCreateDto productDto)
        {
            var vendorId = GetCurrentVendorId();
            var result = await _serviceManager.ProductService.CreateProductAsync(productDto, vendorId);
            return CreatedAtAction(nameof(GetProduct), new { id = result.Id }, result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetProduct(int id)
        {
            var product = await _serviceManager.ProductService.GetProductByIdAsync(id);
            return Ok(product);
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Vendor")]
        public async Task<IActionResult> UpdateProduct(int id, [FromForm] ProductUpdateDto productDto)
        {
            if (id != productDto.Id)
                return BadRequest("ID mismatch");

            await _serviceManager.ProductService.UpdateProductAsync(productDto);
            return NoContent();
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Vendor")]
        public async Task<IActionResult> DeleteProduct(int id)
        {
            await _serviceManager.ProductService.DeleteProductAsync(id);
            return NoContent();
        }
    [HttpGet("vendor")]
    [Authorize(Roles = "Vendor")]
    public async Task<IActionResult> GetVendorProducts()
    {
        try
        {
            var vendorId = GetCurrentVendorId();

            // احصل على جميع المنتجات الخاصة بهذا الـ Vendor
            var products = await _serviceManager.ProductService.GetVendorProductsAsync(vendorId);

            return Ok(products);
        }
        catch (Exception ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    private Guid GetCurrentVendorId()
    {
        var userId = _httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (string.IsNullOrEmpty(userId) || !Guid.TryParse(userId, out var userGuid))
        {
            throw new UnauthorizedAccessException("Invalid user ID");
        }

        // البحث عن الـ Vendor باستخدام الـ User ID
        var vendor = _serviceManager.VendorService.GetVendorByUserIdAsync(userGuid).Result;

        if (vendor == null)
        {
            throw new UnauthorizedAccessException("Vendor profile not found for this user");
        }

        return vendor.Id; // إرجاع الـ Vendor ID وليس الـ User ID
    }
}