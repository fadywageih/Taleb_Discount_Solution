namespace Shared.Dtos.Transaction
{
    public class TransactionCreateDto
    {
        public int ProductId { get; set; }
        public int Quantity { get; set; } = 1;
        public string? DiscountCode { get; set; }

        public bool Validate()
        {
            return ProductId > 0 && Quantity > 0;
        }
    }
}
