namespace Services.MappingProfile
{
    public class ProductProfile : Profile
    {
        public ProductProfile()
        {
            CreateMap<Product, ProductResultDto>()
            .ForMember(d => d.CategoryName, o => o.MapFrom(s => s.ProductCategory.Name))
            .ForMember(d => d.VendorName, o => o.MapFrom(s => s.Vendor.BusinessName))
            .ForMember(d => d.PictureUrl, o => o.MapFrom<PictureUrlResolver>());

            CreateMap<ProductCreateDto, Product>();
            CreateMap<ProductUpdateDto, Product>();
            CreateMap<ProductCategory, CategoryResultDto>();
        }
    }
}