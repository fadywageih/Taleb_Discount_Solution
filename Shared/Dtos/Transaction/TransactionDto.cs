namespace Shared.Dtos.Transaction
{
    public class TransactionDto
    {
        public Guid Id { get; set; }
        public string TransactionId { get; set; }
        public Guid? DiscountCode { get; set; }
        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public string ProductPictureUrl { get; set; }
        public Guid VendorId { get; set; }
        public string VendorName { get; set; }
        public Guid CustomerId { get; set; }
        public string CustomerName { get; set; }
        public string CustomerEmail { get; set; }
        public DateTime TransactionDate { get; set; }
        public decimal Price { get; set; }
        public int Quantity { get; set; }
        public decimal TotalAmount { get; set; }
        public decimal CommissionAmount { get; set; }
        public decimal VendorEarnings { get; set; }
        public string Status { get; set; }
        public string? RejectionReason { get; set; }
        public DateTime? AcceptedDate { get; set; }
        public DateTime? RejectedDate { get; set; }
    }
}
