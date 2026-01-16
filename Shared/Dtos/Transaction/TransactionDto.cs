namespace Shared.Dtos.Transaction
{
    public class TransactionDto
    {
        public Guid Id { get; set; }
        public string TransactionNumber { get; set; } = string.Empty;
        public string? DiscountCode { get; set; }
        public int ProductId { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public string ProductPictureUrl { get; set; } = string.Empty;
        public Guid VendorId { get; set; }
        public string VendorName { get; set; } = string.Empty;
        public string VendorLogoUrl { get; set; } = string.Empty;
        public Guid CustomerId { get; set; }
        public string CustomerName { get; set; } = string.Empty;
        public string CustomerEmail { get; set; } = string.Empty;
        public DateTime TransactionDate { get; set; }
        public decimal Price { get; set; }
        public int Quantity { get; set; }
        public decimal TotalAmount { get; set; }
        public decimal CommissionAmount { get; set; }
        public decimal VendorEarnings { get; set; }
        public string Status { get; set; } = string.Empty;
        public string? RejectionReason { get; set; }
        public DateTime? AcceptedDate { get; set; }
        public DateTime? RejectedDate { get; set; }
        public DateTime? CompletedDate { get; set; }
        public string FormattedTotalAmount => TotalAmount.ToString("C");
        public string FormattedDate => TransactionDate.ToString("dd MMM yyyy HH:mm");
        public string StatusBadgeColor => GetStatusColor();

        private string GetStatusColor()
        {
            return Status.ToLower() switch
            {
                "pending" => "bg-yellow-100 text-yellow-800",
                "accepted" => "bg-blue-100 text-blue-800",
                "completed" => "bg-green-100 text-green-800",
                "rejected" => "bg-red-100 text-red-800",
                _ => "bg-gray-100 text-gray-800"
            };
        }
    }
}
