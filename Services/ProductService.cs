namespace Services
{
    public class ProductService : IProductService
    {
        private readonly IProductRepository _productRepository;
        private readonly IVendorRepository _vendorRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IExtendedImageService _imageService;
        private readonly ILogger<ProductService> _logger;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public ProductService(
            IProductRepository productRepository,
            IVendorRepository vendorRepository,
            IUnitOfWork unitOfWork,
            IMapper mapper,
            IExtendedImageService imageService,
            ILogger<ProductService> logger,
                        IHttpContextAccessor httpContextAccessor)
        {
            _productRepository = productRepository;
            _vendorRepository = vendorRepository;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _imageService = imageService;
            _logger = logger;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<PaginatedResult<ProductResultDto>> GetAllProductsAsync(ProductParameterSpecifications productParameter)
        {
            _logger.LogInformation("Getting all products with parameters: {@Parameters}", productParameter);
            var products = await _productRepository
                .GetAllAsync(new ProductWithCategorySpecifications(productParameter));
            var totalCount = await _productRepository
                .CountAsync(new ProductCountSpecifcations(productParameter));
            var result = _mapper.Map<IEnumerable<ProductResultDto>>(products);
            _logger.LogInformation("Retrieved {Count} products out of {TotalCount}", result.Count(), totalCount);
            return new PaginatedResult<ProductResultDto>(
                productParameter.PageSize,
                productParameter.PageIndex,
                totalCount,
                result
            );
        }
        public async Task<IEnumerable<ProductResultDto>> GetVendorProductsAsync(Guid vendorId)
        {
            var products = await _productRepository.GetProductsByVendorAsync(vendorId);
            return _mapper.Map<IEnumerable<ProductResultDto>>(products);
        }
        public async Task<ProductResultDto> GetProductByIdAsync(int id)
        {
            var product = await _productRepository.GetByIdWithDetailsAsync(id);

            if (product == null)
                throw new ProductNotFoundException(id);

            return _mapper.Map<ProductResultDto>(product);
        }
        public async Task<ProductResultDto> CreateProductAsync(ProductCreateDto productDto, Guid vendorId)
        {
            var vendor = await _vendorRepository.GetVendorByIdAsync(vendorId);
            if (vendor == null)
                throw new VendorNotFoundException(vendorId);
            var category = await _unitOfWork.GetRepository<ProductCategory, int>()
                .GetByIdAsync(productDto.CategoryId);
            if (category == null)
                throw new CategoryNotFoundException(productDto.CategoryId);
            var isNameExist = await _productRepository
                .IsProductNameExistAsync(productDto.Name, vendorId);
            if (isNameExist)
                throw new InvalidOperationException($"Product with name '{productDto.Name}' already exists for this vendor");
            var product = _mapper.Map<Product>(productDto);
            product.VendorId = vendorId;
            if (productDto.Image != null)
            {
                product.PictureUrl = await _imageService.SaveImageAsync(productDto.Image, "products");
            }
            var createdProduct = await _productRepository.AddAsync(product);

            return await GetProductByIdAsync(createdProduct.Id);
        }
        public async Task UpdateProductAsync(ProductUpdateDto productDto)
        {
            var product = await _productRepository.GetByIdAsync(productDto.Id);
            if (product == null)
                throw new ProductNotFoundException(productDto.Id);
            if (productDto.CategoryId != product.CategoryId)
            {
                var category = await _unitOfWork.GetRepository<ProductCategory, int>()
                    .GetByIdAsync(productDto.CategoryId);
                if (category == null)
                    throw new CategoryNotFoundException(productDto.CategoryId);
            }
            _mapper.Map(productDto, product);

            if (productDto.Image != null)
            {
                if (!string.IsNullOrEmpty(product.PictureUrl))
                {
                    await _imageService.DeleteImageAsync(product.PictureUrl);
                }
                product.PictureUrl = await _imageService.SaveImageAsync(productDto.Image, "products");
            }
            await _productRepository.UpdateAsync(product);
        }
        public async Task DeleteProductAsync(int id)
        {
            var product = await _productRepository.GetByIdAsync(id);
            if (product == null)
                throw new ProductNotFoundException(id);
            if (!string.IsNullOrEmpty(product.PictureUrl))
            {
                await _imageService.DeleteImageAsync(product.PictureUrl);
            }
            await _productRepository.DeleteAsync(id);
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
        public async Task EnsureCategoriesSeededAsync()
        {
            var existingCategories = await _unitOfWork.GetRepository<ProductCategory, int>().GetAllAsync();

            if (!existingCategories.Any())
            {
                var categories = new List<ProductCategory>
                {
                    new ProductCategory { Name = "Supplies" },
                    new ProductCategory { Name = "Technology" },
                    new ProductCategory { Name = "Medical" },
                    new ProductCategory { Name = "Engineering" },
                    new ProductCategory { Name = "Workspaces" },
                    new ProductCategory { Name = "Uniform" }
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
            var products = await _productRepository.GetProductsByVendorAsync(vendorId);
            return _mapper.Map<IEnumerable<ProductResultDto>>(products);
        }
        public async Task<IEnumerable<ProductResultDto>> GetActiveProductsAsync()
        {
            var products = await _productRepository.GetActiveProductsAsync();
            return _mapper.Map<IEnumerable<ProductResultDto>>(products);
        }
        public async Task<ProductResultDto> ToggleProductStatusAsync(int id)
        {
            var product = await _productRepository.GetByIdAsync(id);
            if (product == null)
                throw new ProductNotFoundException(id);

            product.IsActive = !product.IsActive;
            await _productRepository.UpdateAsync(product);

            return await GetProductByIdAsync(id);
        }
     public async Task<Guid> GetCurrentVendorIdAsync()
        {
            var userId = _httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userId) || !Guid.TryParse(userId, out var userGuid))
            {
                throw new UnauthorizedAccessException("Invalid user ID");
            }
            var vendor = await _vendorRepository.GetVendorByUserIdAsync(userGuid);
            if (vendor == null)
            {
                throw new UnauthorizedAccessException("Vendor profile not found for this user");
            }
            return vendor.Id;
        }
        public async Task<IEnumerable<ProductResultDto>> GetBestSellingProductsAsync()
        {
            var parameters = new ProductParameterSpecifications
            {
                PageIndex = 1,
                PageSize = 8,
                Sort = ProductSortOption.DiscountDesc
            };

            var result = await GetAllProductsAsync(parameters);
            return result.Data ?? Enumerable.Empty<ProductResultDto>();
        }
        public async Task<IEnumerable<ProductResultDto>> GetProductsByCategoryAsync(int categoryId)
        {
            var products = await _productRepository.GetProductsByCategoryAsync(categoryId);
            return _mapper.Map<IEnumerable<ProductResultDto>>(products);
        }
    }
}