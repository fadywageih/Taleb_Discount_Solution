namespace Services.MappingProfile
{
    public class TransactionProfile : Profile
    {
        public TransactionProfile()
        {
            CreateMap<Transaction, TransactionDto>()
                .ForMember(dest => dest.ProductPictureUrl,
                    opt => opt.MapFrom(src => src.ProductPictureUrl ?? "assets/Images/default-product.jpg"))
                .ForMember(dest => dest.VendorLogoUrl,
                    opt => opt.MapFrom(src => src.Vendor.LogoUrl ?? "assets/Images/default-vendor.jpg"))
                .ForMember(dest => dest.Status,
                    opt => opt.MapFrom(src => src.Status.ToString()));
        }
    }
}
