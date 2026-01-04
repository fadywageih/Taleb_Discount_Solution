namespace Services.MappingProfile
{
    public class FeedBackProfile : Profile
    {
        public FeedBackProfile()
        {
            CreateMap<FeedBackCreateDto, FeedBack>()
                .ForMember(dest => dest.Suggestions,
                    opt => opt.MapFrom(src => src.Suggestions ?? string.Empty));
            CreateMap<FeedBack, FeedBackDto>();
        }
    }
}
