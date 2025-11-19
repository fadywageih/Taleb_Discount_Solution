namespace Domain.Exceptions
{
    public sealed class BrandNotFoundException : NotFoundException
    {
        public BrandNotFoundException(int id) : base($"Brand with id: {id} not found") { }
    }
}
