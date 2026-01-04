namespace Shared.Dtos.FeedBack
{
    public class FeedBackStatsDto
    {
        public int TotalFeedBacks { get; set; }
        public double AverageRating { get; set; }
        public Dictionary<string, int> CategoryCounts { get; set; } = new();
        public Dictionary<int, int> RatingDistribution { get; set; } = new();
    }
}
