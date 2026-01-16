using Shared;
using Shared.Dtos.Product;

namespace ServicesAbstraction
{
    public interface IProductService
    {
        Task<Guid> GetCurrentVendorIdAsync();
        Task<IEnumerable<ProductResultDto>> GetBestSellingProductsAsync();
        Task<IEnumerable<ProductResultDto>> GetProductsByCategoryAsync(int categoryId);
        Task<PaginatedResult<ProductResultDto>> GetAllProductsAsync(ProductParameterSpecifications productParameter);
        Task<IEnumerable<ProductResultDto>> GetVendorProductsAsync(Guid vendorId);
        Task<IEnumerable<CategoryResultDto>> GetAllCategoriesAsync();
        Task<IEnumerable<BrandResultDto>> GetAllBrandAsync();
        Task<ProductResultDto> GetProductByIdAsync(int Id);
        Task<ProductResultDto> CreateProductAsync(ProductCreateDto productDto, Guid vendorId);
        Task UpdateProductAsync(ProductUpdateDto productDto);
        Task DeleteProductAsync(int id);
        Task<IEnumerable<ProductResultDto>> GetProductsByVendorAsync(Guid vendorId);
    }
}