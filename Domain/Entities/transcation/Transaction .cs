using Domain.Entities.User;

namespace Domain.Entities.transcation
{
    public class Transaction : BaseEntity<Guid>
    {
        public string TransactionNumber { get; set; } = string.Empty;
        public string? DiscountCode { get; set; }
        public int ProductId { get; set; }
        public virtual Product.Product Product { get; set; } = null!;
        public string ProductName { get; set; } = string.Empty;
        public string? ProductPictureUrl { get; set; }
        public Guid VendorId { get; set; }
        public virtual User.Vendor Vendor { get; set; } = null!;
        public string VendorName { get; set; } = string.Empty;
        public Guid CustomerId { get; set; }
        public virtual ApplicationUser Customer { get; set; } = null!;
        public string CustomerName { get; set; } = string.Empty;
        public string CustomerEmail { get; set; } = string.Empty;
        public DateTime TransactionDate { get; set; }
        public decimal Price { get; set; }
        public int Quantity { get; set; }
        public decimal TotalAmount => Price * Quantity;
        public decimal CommissionRate { get; set; } = 0.10m;
        public decimal CommissionAmount => TotalAmount * CommissionRate;
        public decimal VendorEarnings => TotalAmount - CommissionAmount;
        public TransactionStatus Status { get; set; } = TransactionStatus.Pending;
        public string? RejectionReason { get; set; }
        public DateTime? AcceptedDate { get; set; }
        public DateTime? RejectedDate { get; set; }
        public DateTime? CompletedDate { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }
    }
}
