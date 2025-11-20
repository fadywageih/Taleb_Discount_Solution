namespace Domain.Entities.Product
{
    public class Product : BaseEntity<int>
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public decimal? DiscountPrice { get; set; }
        public int Quantity { get; set; }
        public int CategoryId { get; set; }
        public ProductCategory ProductCategory { get; set; }

        // إزالة هذين الحقلين تماماً
        // public int BrandId { get; set; }
        // public ProductBrand ProductBrand { get; set; }

        public string Address { get; set; }
        public DateTime? RestockDueDate { get; set; }
        public bool IsActive { get; set; } = true;
        public string PictureUrl { get; set; }
        public Guid VendorId { get; set; }
        public User.Vendor Vendor { get; set; }
    }
}