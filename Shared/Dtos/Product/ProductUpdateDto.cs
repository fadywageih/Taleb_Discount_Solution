using Microsoft.AspNetCore.Http;

namespace Shared.Dtos.Product
{
    public record ProductUpdateDto 
    {
        public int Id { get; init; }
        public string Name { get; init; }
        public string Description { get; init; }
        public decimal Price { get; init; }
        public decimal? DiscountPrice { get; init; }
        public int Quantity { get; init; }
        public int CategoryId { get; init; }
        public string Address { get; init; }
        public DateTime? RestockDueDate { get; init; }
        public bool IsActive { get; init; } = true;
        public IFormFile? Image { get; init; }
    }
}
