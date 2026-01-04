namespace Shared.Dtos.Product
{
    public record ProductResultDto
    {
        public int Id { get; init; }
        public string Name { get; init; }
        public string Description { get; init; }
        public decimal Price { get; init; }
        public decimal? DiscountPrice { get; init; }
        public int Quantity { get; init; }
        public string CategoryName { get; init; }
        public string VendorName { get; init; }
        public string Address { get; init; }
        public DateTime? RestockDueDate { get; init; }
        public bool IsActive { get; init; }
        public string PictureUrl { get; init; }

    }
}
