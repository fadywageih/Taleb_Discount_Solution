namespace Domain.Entities.FeedBack
{
    public class FeedBack : BaseEntity<Guid>
    {
        public string Email { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty; // Vendor, Product, Website
        public int Rating { get; set; }
        public string? Suggestions { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
