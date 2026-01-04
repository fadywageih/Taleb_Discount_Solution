namespace Domain.Exceptions
{
    public sealed class VendorNotFoundException : NotFoundException
    {
        public VendorNotFoundException(Guid id) : base($"Vendor with id: {id} not found") { }
    }
}
