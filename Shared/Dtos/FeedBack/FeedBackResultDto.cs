namespace Shared.Dtos.FeedBack
{
    public class FeedbackResultDto
    {
        public Guid Id { get; set; }
        public string Email { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
        public int Rating { get; set; }
        public string? Suggestions { get; set; }
        public DateTime CreatedAt { get; set; }
        public string? UserName { get; set; }
    }
}
