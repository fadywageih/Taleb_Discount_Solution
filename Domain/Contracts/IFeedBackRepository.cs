using Domain.Entities.FeedBack;

namespace Domain.Contracts
{
    public interface IFeedBackRepository : IGenericRepository<FeedBack, Guid>
    {
        Task<IEnumerable<FeedBack>> GetFeedBacksByEmailAsync(string email);
        Task<IEnumerable<FeedBack>> GetFeedBacksByCategoryAsync(string category);
        Task<(int TotalFeedBacks, double AverageRating,
              Dictionary<string, int> CategoryCounts,
              Dictionary<int, int> RatingDistribution)> GetFeedBackStatisticsAsync();
        Task<IEnumerable<FeedBack>> GetRecentFeedBacksAsync(int count = 10);
    }
}