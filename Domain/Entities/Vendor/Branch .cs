namespace Domain.Entities.Vendor
{
    public class Branch : BaseEntity<Guid>
    {
        public string Address { get; set; } = string.Empty;
        public string City { get; set; } = string.Empty;
        public string State { get; set; } = string.Empty;
        public string? PhoneNumber { get; set; }
        public Guid VendorId { get; set; }
        public User.Vendor Vendor { get; set; } = null!;
    }
}
