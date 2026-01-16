using Domain.Contracts;
using Domain.Entities.Product;
using Microsoft.EntityFrameworkCore;
using Persistance.Data;
namespace Persistance.Repositories
{
    public class ProductRepository : IProductRepository
    {
        private readonly ApplicationDbContext _context;
        public ProductRepository(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<Product?> GetByIdAsync(int id)
        {
            return await _context.Products.FindAsync(id);
        }
        public async Task<Product?> GetByIdWithDetailsAsync(int id)
        {
            return await _context.Products
                .Include(p => p.ProductCategory)
                .Include(p => p.Vendor)
                .ThenInclude(v => v.User)
                .FirstOrDefaultAsync(p => p.Id == id);
        }
        public async Task<IEnumerable<Product>> GetAllAsync(bool asNoTracking = false)
        {
            var query = _context.Products.AsQueryable();

            if (asNoTracking)
                query = query.AsNoTracking();

            return await query.ToListAsync();
        }
        public async Task<IEnumerable<Product>> GetAllWithDetailsAsync()
        {
            return await _context.Products
                .Include(p => p.ProductCategory)
                .Include(p => p.Vendor)
                .ThenInclude(v => v.User)
                .ToListAsync();
        }
        public async Task<Product> AddAsync(Product product)
        {
            await _context.Products.AddAsync(product);
            await _context.SaveChangesAsync();
            return product;
        }
        public async Task UpdateAsync(Product product)
        {
            _context.Products.Update(product);
            await _context.SaveChangesAsync();
        }
        public async Task DeleteAsync(int id)
        {
            var product = await _context.Products.FindAsync(id);
            if (product != null)
            {
                _context.Products.Remove(product);
                await _context.SaveChangesAsync();
            }
        }
        public async Task<IEnumerable<Product>> GetProductsByVendorAsync(Guid vendorId)
        {
            return await _context.Products
                .Where(p => p.VendorId == vendorId)
                .Include(p => p.ProductCategory)
                .Include(p => p.Vendor)
                .OrderByDescending(p => p.Id)
                .ToListAsync();
        }
        public async Task<IEnumerable<Product>> GetProductsByCategoryAsync(int categoryId)
        {
            return await _context.Products
                .Where(p => p.CategoryId == categoryId && p.IsActive)
                .Include(p => p.ProductCategory)
                .Include(p => p.Vendor)
                .OrderByDescending(p => p.Id)
                .ToListAsync();
        }
        public async Task<IEnumerable<Product>> GetActiveProductsAsync()
        {
            return await _context.Products
                .Where(p => p.IsActive)
                .Include(p => p.ProductCategory)
                .Include(p => p.Vendor)
                .OrderByDescending(p => p.Id)
                .ToListAsync();
        }
        public async Task<bool> IsProductNameExistAsync(string name, Guid vendorId)
        {
            return await _context.Products
                .AnyAsync(p => p.Name.ToLower() == name.ToLower() && p.VendorId == vendorId);
        }
        public async Task<int> GetProductCountByVendorAsync(Guid vendorId)
        {
            return await _context.Products
                .CountAsync(p => p.VendorId == vendorId);
        }
        public async Task<IEnumerable<Product>> GetAllAsync(Specifications<Product> specifications)
        {
            var query = ApplySpecifications(specifications);
            return await query.ToListAsync();
        }
        public async Task<int> CountAsync(Specifications<Product> specifications)
        {
            var query = ApplySpecifications(specifications);
            return await query.CountAsync();
        }
        private IQueryable<Product> ApplySpecifications(Specifications<Product> specifications)
        {
            return SpecificationsEvaluator.GetQuery(_context.Products.AsQueryable(), specifications);
        }
    }
}