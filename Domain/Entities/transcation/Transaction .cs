using Domain.Entities.User;

namespace Domain.Entities.transcation
{
    public class Transaction : BaseEntity<Guid>
    {
        public string TransactionId { get; set; } // مثل: TRX-2023-05842
        public Guid? DiscountCode { get; set; }
        public int ProductId { get; set; }
        public Product.Product Product { get; set; }
        public Guid VendorId { get; set; }
        public User.Vendor Vendor { get; set; }
        public Guid CustomerId { get; set; } // إضافة العميل
        public ApplicationUser Customer { get; set; } // إضافة علاقة بالعميل
        public DateTime TransactionDate { get; set; }
        public decimal Price { get; set; }
        public int Quantity { get; set; }
        public decimal TotalAmount => Price * Quantity;
        public decimal CommissionRate { get; set; } = 0.10m; // 10% commission
        public decimal CommissionAmount => TotalAmount * CommissionRate;
        public decimal VendorEarnings => TotalAmount - CommissionAmount;
        public TransactionStatus Status { get; set; } = TransactionStatus.Pending;
        public string? RejectionReason { get; set; }
        public DateTime? AcceptedDate { get; set; }
        public DateTime? RejectedDate { get; set; }
    }
}
