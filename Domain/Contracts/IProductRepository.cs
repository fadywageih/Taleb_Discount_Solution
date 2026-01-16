using Domain.Entities.Product;

namespace Domain.Contracts
{
    public interface IProductRepository
    {
        Task<Product?> GetByIdAsync(int id);
        Task<Product?> GetByIdWithDetailsAsync(int id);
        Task<IEnumerable<Product>> GetAllAsync(bool asNoTracking = false);
        Task<IEnumerable<Product>> GetAllWithDetailsAsync();
        Task<Product> AddAsync(Product product);
        Task UpdateAsync(Product product);
        Task DeleteAsync(int id);

        Task<IEnumerable<Product>> GetProductsByVendorAsync(Guid vendorId);
        Task<IEnumerable<Product>> GetProductsByCategoryAsync(int categoryId);
        Task<IEnumerable<Product>> GetActiveProductsAsync();
        Task<bool> IsProductNameExistAsync(string name, Guid vendorId);
        Task<int> GetProductCountByVendorAsync(Guid vendorId);

        Task<IEnumerable<Product>> GetAllAsync(Specifications<Product> specifications);
        Task<int> CountAsync(Specifications<Product> specifications);
    }
}
