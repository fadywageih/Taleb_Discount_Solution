namespace Services
{
    public class FeedBackService : IFeedBackService
    {
        private readonly IFeedBackRepository _feedBackRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        public FeedBackService(
            IFeedBackRepository feedBackRepository,
            IUnitOfWork unitOfWork,
            IMapper mapper)
        {
            _feedBackRepository = feedBackRepository;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }
        public async Task<FeedBackDto> CreateFeedBackAsync(FeedBackCreateDto dto)
        {
            if (dto.Rating < 1 || dto.Rating > 5)
                throw new ArgumentException("Rating must be between 1 and 5");
            var validCategories = new[] { "Vendor", "Product", "Website" };
            if (!validCategories.Contains(dto.Category))
                throw new ArgumentException($"Invalid category. Must be one of: {string.Join(", ", validCategories)}");
            var feedBack = _mapper.Map<FeedBack>(dto);
            feedBack.CreatedAt = DateTime.UtcNow;
            await _feedBackRepository.AddAsync(feedBack);
            await _unitOfWork.SaveChangesAsync();
            return _mapper.Map<FeedBackDto>(feedBack);
        }
        public async Task<FeedBackDto?> GetFeedBackByIdAsync(Guid id)
        {
            var feedBack = await _feedBackRepository.GetByIdAsync(id);
            return feedBack == null ? null : _mapper.Map<FeedBackDto>(feedBack);
        }
        public async Task<IEnumerable<FeedBackDto>> GetFeedBacksByEmailAsync(string email)
        {
            var feedBacks = await _feedBackRepository.GetFeedBacksByEmailAsync(email);
            return _mapper.Map<IEnumerable<FeedBackDto>>(feedBacks);
        }
        public async Task<FeedBackStatsDto> GetFeedBackStatisticsAsync()
        {
            var (totalFeedBacks, averageRating, categoryCounts, ratingDistribution) =
                await _feedBackRepository.GetFeedBackStatisticsAsync();
            return new FeedBackStatsDto
            {
                TotalFeedBacks = totalFeedBacks,
                AverageRating = averageRating,
                CategoryCounts = categoryCounts,
                RatingDistribution = ratingDistribution
            };
        }
        public async Task<IEnumerable<FeedBackDto>> GetRecentFeedBacksAsync(int count = 10)
        {
            var feedBacks = await _feedBackRepository.GetRecentFeedBacksAsync(count);
            return _mapper.Map<IEnumerable<FeedBackDto>>(feedBacks);
        }
    }
}