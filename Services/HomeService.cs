using Domain.Contracts;
using Services.Specifications;
using Shared.Dtos.Product;
using Shared.Dtos.User.Shared.Dtos.Home;
using Shared.Dtos.User.Shared.Dtos.Vendor;

namespace Services
{
    public class HomeService : IHomeService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IProductService _productService;

        public HomeService(IUnitOfWork unitOfWork, IMapper mapper, IProductService productService)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _productService = productService;
        }

        public async Task<HomePageDto> GetHomePageDataAsync()
        {
            var homeData = new HomePageDto
            {
                FeaturedProducts = await GetFeaturedProductsAsync(),
                Categories = await _productService.GetAllCategoriesAsync(),
                FeaturedVendors = await GetFeaturedVendorsAsync(),
                Advertisements = await GetActiveAdvertisementsAsync()
            };

            return homeData;
        }

        public async Task<HomePageDto> GetHomePageDataForStudentAsync(string userType)
        {
            var homeData = await GetHomePageDataAsync();

            // يمكننا إضافة منطق خاص بنوع الطالب إذا احتجنا
            // مثلاً ترتيب مختلف أو منتجات مخصصة
            if (userType == "School")
            {
                homeData.FeaturedProducts = homeData.FeaturedProducts
                    .Where(p => p.CategoryName != "Technology") // مثال
                    .ToList();
            }

            return homeData;
        }

        private async Task<IEnumerable<ProductResultDto>> GetFeaturedProductsAsync()
        {
            var productSpecs = new ProductParameterSpecifications
            {
                PageSize = 8, // عدد المنتجات المميزة
                PageIndex = 1
            };

            var paginatedProducts = await _productService.GetAllProductsAsync(productSpecs);
            return paginatedProducts.Data.Take(8); // نأخذ أول 8 منتجات
        }

        private async Task<IEnumerable<VendorLogoDto>> GetFeaturedVendorsAsync()
        {
            var vendors = await _unitOfWork.GetRepository<Vendor, Guid>()
                .GetAllAsync(new VendorWithUserSpecification());

            return vendors
                .Where(v => !string.IsNullOrEmpty(v.LogoUrl))
                .Take(6) // عدد الفيندورز المميزين
                .Select(v => new VendorLogoDto
                {
                    Id = v.Id,
                    BusinessName = v.BusinessName,
                    LogoUrl = v.LogoUrl
                })
                .ToList();
        }

        private async Task<IEnumerable<AdvertisementDto>> GetActiveAdvertisementsAsync()
        {
            // هنا يمكن جلب الإعلانات من قاعدة البيانات
            // حالياً هعمل إعلانات static

            return new List<AdvertisementDto>
            {
                new AdvertisementDto
                {
                    Id = 1,
                    Title = "Back to School Sale!",
                    Description = "Get up to 50% off on all school supplies",
                    ImageUrl = "/images/ads/school-sale.jpg",
                    TargetUrl = "/products?category=Supplies",
                    IsActive = true
                },
                new AdvertisementDto
                {
                    Id = 2,
                    Title = "New Tech Arrivals",
                    Description = "Latest laptops and tablets for students",
                    ImageUrl = "/images/ads/tech-sale.jpg",
                    TargetUrl = "/products?category=Technology",
                    IsActive = true
                }
            };
        }
    }
}