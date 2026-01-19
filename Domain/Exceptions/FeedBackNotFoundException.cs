namespace Domain.Exceptions
{
    public sealed class FeedBackNotFoundException : NotFoundException
        {
        public FeedBackNotFoundException(Guid id) : base($"FeedBack with id: {id} not found") { }
        public FeedBackNotFoundException(string email) : base($"FeedBack with email: {email} not found") { }
        }

}
