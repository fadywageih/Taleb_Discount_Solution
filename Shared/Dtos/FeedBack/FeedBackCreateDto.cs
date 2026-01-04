namespace Shared.Dtos.FeedBack
{
    public class FeedBackCreateDto
    {
        public string Email { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
        public int Rating { get; set; }
        public string? Suggestions { get; set; }
    }
}
