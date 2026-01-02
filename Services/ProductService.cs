using Domain.Contracts;
using Domain.Entities.Product;
using Domain.Exceptions;
using Services.Specifications;
using ServicesAbstraction;
using Shared.Dtos.Product;
using Domain.Entities.User;

namespace Services
{
    public class ProductService : IProductService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IExtendedImageService _imageService;

        public ProductService(IUnitOfWork unitOfWork, IMapper mapper, IExtendedImageService imageService)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _imageService = imageService;
        }

        public async Task<ProductResultDto> CreateProductAsync(ProductCreateDto productDto, Guid vendorId)
        {
            // تأكد من وجود الـ categories
            await EnsureCategoriesSeededAsync();

            var vendor = await _unitOfWork.GetRepository<Vendor, Guid>().GetByIdAsync(vendorId);
            if (vendor == null)
                throw new VendorNotFoundException(vendorId);

            var category = await _unitOfWork.GetRepository<ProductCategory, int>().GetByIdAsync(productDto.CategoryId);
            if (category == null)
                throw new CategoryNotFoundException(productDto.CategoryId);

            var product = _mapper.Map<Product>(productDto);
            product.VendorId = vendorId;

            // لا يوجد BrandId الآن

            if (productDto.Image != null)
            {
                product.PictureUrl = await _imageService.SaveImageAsync(productDto.Image, "products");
            }

            await _unitOfWork.GetRepository<Product, int>().AddAsync(product);
            await _unitOfWork.SaveChangesAsync();

            var createdProduct = await _unitOfWork.GetRepository<Product, int>()
                .GetByIdAsync(new ProductWithCategorySpecifications(product.Id));

            return _mapper.Map<ProductResultDto>(createdProduct);
        }
        public async Task<IEnumerable<ProductResultDto>> GetVendorProductsAsync(Guid vendorId)
        {
            var products = await _unitOfWork.GetRepository<Product, int>()
                .GetAllAsync(new ProductWithCategorySpecifications(vendorId));

            return _mapper.Map<IEnumerable<ProductResultDto>>(products);
        }
        public async Task UpdateProductAsync(ProductUpdateDto productDto)
        {
            var product = await _unitOfWork.GetRepository<Product, int>().GetByIdAsync(productDto.Id);
            if (product == null)
                throw new ProductNotFoundException(productDto.Id);

            _mapper.Map(productDto, product);

            if (productDto.Image != null)
            {
                if (!string.IsNullOrEmpty(product.PictureUrl))
                {
                    await _imageService.DeleteImageAsync(product.PictureUrl);
                }
                product.PictureUrl = await _imageService.SaveImageAsync(productDto.Image, "products");
            }

            _unitOfWork.GetRepository<Product, int>().Update(product);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task DeleteProductAsync(int id)
        {
            var product = await _unitOfWork.GetRepository<Product, int>().GetByIdAsync(id);
            if (product == null)
                throw new ProductNotFoundException(id);

            if (!string.IsNullOrEmpty(product.PictureUrl))
            {
                await _imageService.DeleteImageAsync(product.PictureUrl);
            }

            _unitOfWork.GetRepository<Product, int>().Delete(product);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task<IEnumerable<CategoryResultDto>> GetAllCategoriesAsync()
        {
            var categories = await _unitOfWork.GetRepository<ProductCategory, int>().GetAllAsync();
            return _mapper.Map<IEnumerable<CategoryResultDto>>(categories);
        }

        public async Task<IEnumerable<BrandResultDto>> GetAllBrandAsync()
        {
            var brands = await _unitOfWork.GetRepository<ProductBrand, int>().GetAllAsync();
            return _mapper.Map<IEnumerable<BrandResultDto>>(brands);
        }

        public async Task<ProductResultDto> GetProductByIdAsync(int id)
        {
            var product = await _unitOfWork.GetRepository<Product, int>()
                .GetByIdAsync(new ProductWithCategorySpecifications(id));

            if (product == null)
                throw new ProductNotFoundException(id);

            return _mapper.Map<ProductResultDto>(product);
        }

        public async Task<PaginatedResult<ProductResultDto>> GetAllProductsAsync(ProductParameterSpecifications productParameter)
        {
            var products = await _unitOfWork.GetRepository<Product, int>()
                .GetAllAsync(new ProductWithCategorySpecifications(productParameter));

            var totalCount = await _unitOfWork.GetRepository<Product, int>()
                .CountAsync(new ProductCountSpecifcations(productParameter));

            var result = _mapper.Map<IEnumerable<ProductResultDto>>(products);

            return new PaginatedResult<ProductResultDto>(
                productParameter.PageSize,
                productParameter.PageIndex,
                totalCount,
                result
            );
        }
        public async Task EnsureCategoriesSeededAsync()
        {
            var existingCategories = await _unitOfWork.GetRepository<ProductCategory, int>().GetAllAsync();

            if (!existingCategories.Any())
            {
                var categories = new List<ProductCategory>
        {
            new ProductCategory { Id = 1, Name = "Supplies" },
            new ProductCategory { Id = 2, Name = "Technology" },
            new ProductCategory { Id = 3, Name = "Medical" },
            new ProductCategory { Id = 4, Name = "Engineering" },
            new ProductCategory { Id = 5, Name = "Workspaces" },
            new ProductCategory { Id = 6, Name = "Uniform" }
        };

                foreach (var category in categories)
                {
                    await _unitOfWork.GetRepository<ProductCategory, int>().AddAsync(category);
                }

                await _unitOfWork.SaveChangesAsync();
            }

        }
        public async Task<IEnumerable<ProductResultDto>> GetProductsByVendorAsync(Guid vendorId)
        {
            var spec = new ProductWithCategorySpecifications(vendorId);
            var products = await _unitOfWork.GetRepository<Product, int>().GetAllAsync(spec);
            return _mapper.Map<IEnumerable<ProductResultDto>>(products);
        }
    }
}