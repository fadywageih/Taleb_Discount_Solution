namespace Services.MappingProfile
{
    public class HomeMappingProfile : Profile
    {
        public HomeMappingProfile()
        {
            CreateMap<Vendor, VendorLogoDto>()
                .ForMember(dest => dest.LogoUrl, opt => opt.MapFrom(src => src.LogoUrl));
        }
    }
}