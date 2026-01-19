using Domain.Contracts;
using Domain.Entities.FeedBack;
using Microsoft.EntityFrameworkCore;
using Persistance.Data;

namespace Persistance.Repositories
{
    public class FeedBackRepository : IFeedBackRepository
    {
        private readonly ApplicationDbContext _context;
        public FeedBackRepository(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<FeedBack?> GetByIdAsync(Guid id)
        {
            return await _context.FeedBacks.FindAsync(id);
        }
        public async Task<FeedBack?> GetByIdAsync(Specifications<FeedBack> specifications)
        {
            var query = ApplySpecification(specifications);
            return await query.FirstOrDefaultAsync();
        }
        public async Task<IEnumerable<FeedBack>> GetAllAsync(bool asNoTracking = false)
        {
            var query = _context.FeedBacks.AsQueryable();
            if (asNoTracking)
                query = query.AsNoTracking();

            return await query.ToListAsync();
        }
        public async Task<IEnumerable<FeedBack>> GetAllAsync(Specifications<FeedBack> specifications)
        {
            var query = ApplySpecification(specifications);
            return await query.ToListAsync();
        }
        public async Task AddAsync(FeedBack entity)
        {
            await _context.FeedBacks.AddAsync(entity);
        }
        public void Update(FeedBack entity)
        {
            _context.FeedBacks.Update(entity);
        }
        public void Delete(FeedBack entity)
        {
            _context.FeedBacks.Remove(entity);
        }
        public async Task<int> CountAsync(Specifications<FeedBack> specifications)
        {
            var query = ApplySpecification(specifications);
            return await query.CountAsync();
        }
        public async Task<IEnumerable<FeedBack>> GetFeedBacksByEmailAsync(string email)
        {
            return await _context.FeedBacks
                .Where(f => f.Email == email)
                .OrderByDescending(f => f.CreatedAt)
                .ToListAsync();
        }
        public async Task<IEnumerable<FeedBack>> GetFeedBacksByCategoryAsync(string category)
        {
            return await _context.FeedBacks
                .Where(f => f.Category == category)
                .OrderByDescending(f => f.CreatedAt)
                .ToListAsync();
        }
        public async Task<(int TotalFeedBacks, double AverageRating,
                           Dictionary<string, int> CategoryCounts,
                           Dictionary<int, int> RatingDistribution)> GetFeedBackStatisticsAsync()
        {
            var feedBacks = await _context.FeedBacks.ToListAsync();

            var totalFeedBacks = feedBacks.Count;
            var averageRating = feedBacks.Any() ? feedBacks.Average(f => f.Rating) : 0;

            var categoryCounts = new Dictionary<string, int>();
            var ratingDistribution = new Dictionary<int, int>();

            var categories = new[] { "Vendor", "Product", "Website" };
            foreach (var category in categories)
            {
                var count = feedBacks.Count(f => f.Category == category);
                categoryCounts.Add(category, count);
            }

            for (int i = 1; i <= 5; i++)
            {
                var count = feedBacks.Count(f => f.Rating == i);
                ratingDistribution.Add(i, count);
            }

            return (totalFeedBacks, averageRating, categoryCounts, ratingDistribution);
        }
        public async Task<IEnumerable<FeedBack>> GetRecentFeedBacksAsync(int count = 10)
        {
            return await _context.FeedBacks
                .OrderByDescending(f => f.CreatedAt)
                .Take(count)
                .ToListAsync();
        }
        private IQueryable<FeedBack> ApplySpecification(Specifications<FeedBack> specifications)
        {
            return SpecificationsEvaluator.GetQuery(_context.FeedBacks.AsQueryable(), specifications);
        }
    }
}