using Shared.Dtos.FeedBack;

namespace ServicesAbstraction
{
    public interface IFeedBackService
    {
        Task<FeedBackDto> CreateFeedBackAsync(FeedBackCreateDto dto);
        Task<FeedBackDto?> GetFeedBackByIdAsync(Guid id);
        Task<IEnumerable<FeedBackDto>> GetFeedBacksByEmailAsync(string email);
        Task<FeedBackStatsDto> GetFeedBackStatisticsAsync();
        Task<IEnumerable<FeedBackDto>> GetRecentFeedBacksAsync(int count = 10);
    }
}
