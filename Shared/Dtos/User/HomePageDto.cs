using Shared.Dtos.Product;
using Shared.Dtos.User.Shared.Dtos.Vendor;


namespace Shared.Dtos.User
{
    namespace Shared.Dtos.Home
    {
        public class HomePageDto
        {
            public IEnumerable<ProductResultDto> FeaturedProducts { get; set; } = new List<ProductResultDto>();
            public IEnumerable<CategoryResultDto> Categories { get; set; } = new List<CategoryResultDto>();
            public IEnumerable<VendorLogoDto> FeaturedVendors { get; set; } = new List<VendorLogoDto>();
            public IEnumerable<AdvertisementDto> Advertisements { get; set; } = new List<AdvertisementDto>();
        }
    }
}
